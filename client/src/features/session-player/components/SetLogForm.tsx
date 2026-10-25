import { useState, type MutableRefObject } from 'react'
import type { CreateSetLogRequest } from '../../../shared/api/types'
import type { SessionStep } from '../session-plan'

interface SetLogFormProps {
  /** The work step that was just completed (the set being logged). */
  workStep: SessionStep
  /** The session's accumulating set-log map, keyed by work-step id. */
  setLogsRef: MutableRefObject<Map<string, CreateSetLogRequest>>
}

interface LastValue {
  weightKg?: number | null
  reps?: number | null
  rpe?: number | null
}

const lastKey = (exerciseId: number | null | undefined) =>
  exerciseId != null ? `ib_lastset_${exerciseId}` : null

function readLast(exerciseId: number | null | undefined): LastValue | null {
  const key = lastKey(exerciseId)
  if (!key) return null
  try {
    const raw = localStorage.getItem(key)
    return raw ? (JSON.parse(raw) as LastValue) : null
  } catch {
    return null
  }
}

function writeLast(exerciseId: number | null | undefined, value: LastValue) {
  const key = lastKey(exerciseId)
  if (!key) return
  try {
    localStorage.setItem(key, JSON.stringify(value))
  } catch {
    /* storage unavailable */
  }
}

/**
 * Captures the actual weight / reps / RPE for the set that just finished. Defaults
 * to the last value logged for the same exercise — the previous set this session,
 * or the last time you did it (persisted) — so you rarely re-type the weight.
 * Writes straight into the session's set-log map and persists the last value.
 */
/**
 * Dumbbell movements are logged as the weight of ONE dumbbell (per hand); barbell
 * and bodyweight movements are a single total. Inferred from the exercise name.
 */
function isPerHand(name: string | undefined): boolean {
  if (!name) return false
  return /\b(dumbbell|hammer curl|lateral raise|rear[- ]?delt|renegade|flye|flyes|one[- ]?arm)\b/i.test(name)
}

export function SetLogForm({ workStep, setLogsRef }: SetLogFormProps) {
  const existing = setLogsRef.current.get(workStep.id)
  const alreadySaved = existing?.weightKg != null
  const perHand = isPerHand(workStep.exerciseName)

  // The most recent set of this exercise with a logged weight (earlier this session).
  const priorThisSession = [...setLogsRef.current.entries()]
    .filter(([k, l]) => k !== workStep.id && l.exerciseId === workStep.exerciseId && l.weightKg != null)
    .map(([, l]) => l)
    .sort((a, b) => (b.setNumber ?? 0) - (a.setNumber ?? 0))[0]

  const fallback: LastValue = priorThisSession
    ? { weightKg: priorThisSession.weightKg, reps: priorThisSession.repsCompleted, rpe: priorThisSession.rpe }
    : (readLast(workStep.exerciseId) ?? {})

  const [weight, setWeight] = useState(
    (alreadySaved ? existing?.weightKg : fallback.weightKg)?.toString() ?? '',
  )
  const [reps, setReps] = useState(
    (alreadySaved ? existing?.repsCompleted : (fallback.reps ?? workStep.plannedReps))?.toString() ?? '',
  )
  const [rpe, setRpe] = useState((alreadySaved ? existing?.rpe : fallback.rpe)?.toString() ?? '')
  const [saved, setSaved] = useState(false)

  const save = () => {
    const base: CreateSetLogRequest = setLogsRef.current.get(workStep.id) ?? {
      exerciseId: workStep.exerciseId ?? null,
      exerciseName: workStep.exerciseName ?? null,
      setNumber: workStep.setNumber ?? 1,
      repsCompleted: workStep.plannedReps ?? null,
    }
    const weightKg = weight ? Number(weight) : null
    const repsCompleted = reps ? Number(reps) : base.repsCompleted
    const rpeVal = rpe ? Number(rpe) : null
    setLogsRef.current.set(workStep.id, { ...base, repsCompleted, weightKg, rpe: rpeVal })
    writeLast(workStep.exerciseId, { weightKg, reps: repsCompleted, rpe: rpeVal })
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
          <span title={perHand ? 'Enter the weight of ONE dumbbell (per hand).' : 'Total weight on the bar / plate.'}>
            {perHand ? 'Weight / hand (kg)' : 'Weight (kg)'}
          </span>
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
