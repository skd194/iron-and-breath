import { addDays, sameDay, toIsoDate } from '../../../shared/util/date'

interface CalendarHeatmapProps {
  /** Set of yyyy-MM-dd strings that had a completed session. */
  completedDates: Set<string>
  days?: number
}

interface Cell {
  iso: string
  done: boolean
}

const WEEKDAY_LABELS = ['M', 'T', 'W', 'T', 'F', 'S', 'S']

/**
 * A GitHub-style heatmap of the last N days (default 35 = 5 weeks), oldest
 * column first, aligned to Monday-anchored weeks.
 */
export function CalendarHeatmap({ completedDates, days = 35 }: CalendarHeatmapProps) {
  const today = new Date()

  const cells: Cell[] = []
  for (let i = days - 1; i >= 0; i--) {
    const d = addDays(today, -i)
    const iso = toIsoDate(d)
    cells.push({ iso, done: completedDates.has(iso) })
  }

  // Pad the front so the first cell lands on the correct Monday-anchored row.
  const firstDate = new Date(cells[0].iso)
  const leadOffset = (firstDate.getDay() + 6) % 7 // Mon=0 … Sun=6
  const padded: (Cell | null)[] = [...Array<null>(leadOffset).fill(null), ...cells]

  // Column-major weeks.
  const weeks: (Cell | null)[][] = []
  for (let i = 0; i < padded.length; i += 7) {
    weeks.push(padded.slice(i, i + 7))
  }

  return (
    <div className="heatmap">
      <div className="heatmap-weekdays">
        {WEEKDAY_LABELS.map((l, i) => (
          <span key={i} className="heatmap-weekday">
            {l}
          </span>
        ))}
      </div>
      <div className="heatmap-grid">
        {weeks.map((week, wi) => (
          <div key={wi} className="heatmap-col">
            {Array.from({ length: 7 }).map((_, di) => {
              const cell = week[di]
              if (!cell) {
                return <span key={di} className="heatmap-cell empty" />
              }
              const isToday = sameDay(new Date(cell.iso), today)
              return (
                <span
                  key={di}
                  className={`heatmap-cell${cell.done ? ' done' : ''}${isToday ? ' today' : ''}`}
                  title={`${cell.iso}${cell.done ? ' — completed' : ''}`}
                />
              )
            })}
          </div>
        ))}
      </div>
    </div>
  )
}
