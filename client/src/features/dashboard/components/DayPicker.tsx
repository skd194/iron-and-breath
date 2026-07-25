import { Link } from 'react-router-dom'
import type { WorkoutDayDto } from '../../../shared/api/types'

interface DayPickerProps {
  days: WorkoutDayDto[]
  suggestedDayId: number | null
  perDayCounts: Record<number, number>
}

export function DayPicker({ days, suggestedDayId, perDayCounts }: DayPickerProps) {
  return (
    <div className="daypicker">
      {days.map((day) => {
        const count = perDayCounts[day.id] ?? 0
        const suggested = day.id === suggestedDayId
        return (
          <Link
            key={day.id}
            to={`/session/${day.id}`}
            className={`daycard${suggested ? ' suggested' : ''}`}
          >
            <div className="daycard-head">
              <span className="daycard-index">Day {day.sortOrder}</span>
              {suggested && <span className="badge badge-accent">Suggested</span>}
            </div>
            <div className="daycard-name">{day.name}</div>
            <div className="daycard-focus">{day.focus}</div>
            <div className="daycard-foot">
              <span className="muted">{day.exercises.length} exercises</span>
              <span className="daycard-count" title="Completed sessions">
                ✓ {count}
              </span>
            </div>
          </Link>
        )
      })}
    </div>
  )
}
