import { addDays, sameDay, toIsoDate } from '../../../shared/util/date'

interface CalendarHeatmapProps {
  /** Set of yyyy-MM-dd strings that had a completed session. */
  completedDates: Set<string>
  /** Set of yyyy-MM-dd strings that were scheduled but missed. */
  missedDates?: Set<string>
  days?: number
}

interface Cell {
  iso: string
  done: boolean
  missed: boolean
}

const WEEKDAY_LABELS = ['M', 'T', 'W', 'T', 'F', 'S', 'S']

/**
 * A GitHub-style heatmap of the last N days (default 35 = 5 weeks), oldest
 * column first, aligned to Monday-anchored weeks. Cells are three-state:
 * completed, missed (scheduled but skipped), or rest/none.
 */
export function CalendarHeatmap({ completedDates, missedDates, days = 35 }: CalendarHeatmapProps) {
  const today = new Date()

  const cells: Cell[] = []
  for (let i = days - 1; i >= 0; i--) {
    const d = addDays(today, -i)
    const iso = toIsoDate(d)
    cells.push({ iso, done: completedDates.has(iso), missed: missedDates?.has(iso) ?? false })
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
              const state = cell.done ? ' done' : cell.missed ? ' missed' : ''
              const label = cell.done ? ' — completed' : cell.missed ? ' — missed' : ''
              return (
                <span
                  key={di}
                  className={`heatmap-cell${state}${isToday ? ' today' : ''}`}
                  title={`${cell.iso}${label}`}
                />
              )
            })}
          </div>
        ))}
      </div>
    </div>
  )
}
