import { useState } from 'react'
import { useUpdateSession } from '../../shared/api/hooks'
import type { SessionDto, WorkoutDayDto } from '../../shared/api/types'

interface EditSessionModalProps {
  session: SessionDto
  days: WorkoutDayDto[]
  onClose: () => void
}

export function EditSessionModal({ session, days, onClose }: EditSessionModalProps) {
  const updateSession = useUpdateSession()
  const [date, setDate] = useState(session.date)
  const [workoutDayId, setWorkoutDayId] = useState(session.workoutDayId)
  const [completed, setCompleted] = useState(session.completedAt != null)

  const save = () => {
    // Preserve the original completion timestamp when it stays completed;
    // stamp now if it's newly marked complete; null when marked abandoned.
    const completedAt = completed ? session.completedAt ?? new Date().toISOString() : null
    updateSession.mutate(
      { id: session.id, body: { workoutDayId, date, completedAt } },
      { onSuccess: onClose },
    )
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal card card-pad" onClick={(e) => e.stopPropagation()}>
        <h2>Edit session</h2>

        <label className="field">
          <span>Date</span>
          <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
        </label>

        <label className="field">
          <span>Workout day</span>
          <select
            value={workoutDayId}
            onChange={(e) => setWorkoutDayId(Number(e.target.value))}
          >
            {days.map((d) => (
              <option key={d.id} value={d.id}>
                {d.name}
              </option>
            ))}
          </select>
        </label>

        <label className="field field-check">
          <input
            type="checkbox"
            checked={completed}
            onChange={(e) => setCompleted(e.target.checked)}
          />
          <span>Completed (uncheck to mark as abandoned)</span>
        </label>

        {updateSession.isError && (
          <p style={{ color: 'var(--danger-500)' }}>Couldn't save changes. Try again.</p>
        )}

        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={onClose}>
            Cancel
          </button>
          <button className="btn btn-primary" disabled={updateSession.isPending} onClick={save}>
            {updateSession.isPending ? 'Saving…' : 'Save'}
          </button>
        </div>
      </div>
    </div>
  )
}
