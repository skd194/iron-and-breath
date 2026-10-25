import { useState, type MutableRefObject } from 'react'
import type { CreateSetLogRequest } from '../../../shared/api/types'
import type { SessionStep } from '../session-plan'

interface SetLogFormProps {
  /** The work step that was just completed (the set being logged). */
  workStep: SessionStep
  /** The session's accumulating set-log map, keyed by work-step id. */
  setLogsRef: MutableRefObject<Map<string, CreateSetLogRequest>>
}

/**
 * Captures the actual weight / reps / RPE for the set that just finished, during
 * the rest that follows it. Writes straight into the session's set-log map so the
 * numbers are saved when the session is logged. Keyed by the work step, so the
 * values persist if the user leaves and returns to this rest step.
 */
export function SetLogForm({ workStep, setLogsRef }: SetLogFormProps) {
  const existing = setLogsRef.current.get(workStep.id)
  const [reps, setReps] = useState(
    existing?.repsCompleted?.toString() ?? workStep.plannedReps?.toString() ?? '',
  )
  const [weight, setWeight] = useState(existing?.weightKg?.toString() ?? '')
  const [rpe, setRpe] = useState(existing?.rpe?.toString() ?? '')
  const [saved, setSaved] = useState(false)

  const save = () => {
    const base: CreateSetLogRequest = setLogsRef.current.get(workStep.id) ?? {
      exerciseId: workStep.exerciseId ?? null,
      exerciseName: workStep.exerciseName ?? null,
      setNumber: workStep.setNumber ?? 1,
      repsCompleted: workStep.plannedReps ?? null,
    }
    setLogsRef.current.set(workStep.id, {
      ...base,
      repsCompleted: reps ? Number(reps) : base.repsCompleted,
      weightKg: weight ? Number(weight) : null,
      rpe: rpe ? Number(rpe) : null,
    })
    setSaved(true)
  }

  const onEdit = (setter: (v: string) => void) => (v: string) => {
    setter(v)
    setSaved(false)
  }

  return (
    <div className="setlog">
      <div className="setlog-head">
        Log {workStep.exerciseName ?? 'set'} · set {workStep.setNumber}
      </div>
      <div className="setlog-fields">
        <label className="setlog-field">
          <span>Weight (kg)</span>
          <input
            type="number"
            inputMode="decimal"
            step="0.5"
            value={weight}
            onChange={(e) => onEdit(setWeight)(e.target.value)}
            placeholder="—"
          />
        </label>
        <label className="setlog-field">
          <span>Reps</span>
          <input
            type="number"
            inputMode="numeric"
            value={reps}
            onChange={(e) => onEdit(setReps)(e.target.value)}
            placeholder={workStep.plannedReps?.toString() ?? '—'}
          />
        </label>
        <label className="setlog-field">
          <span>RPE</span>
          <input
            type="number"
            inputMode="numeric"
            min="1"
            max="10"
            value={rpe}
            onChange={(e) => onEdit(setRpe)(e.target.value)}
            placeholder="1-10"
          />
        </label>
      </div>
      <button className="btn btn-primary btn-sm setlog-save" onClick={save}>
        {saved ? 'Saved ✓' : 'Save set'}
      </button>
    </div>
  )
}
