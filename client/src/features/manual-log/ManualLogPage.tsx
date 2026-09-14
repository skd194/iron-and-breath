import { useMemo, useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateSession, useWorkoutDays } from '../../shared/api/hooks'
import type { CreateSessionRequest, CreateSetLogRequest, ExerciseDto } from '../../shared/api/types'
import { EmptyState, LoadingState } from '../../shared/ui/States'
import { todayIso } from '../../shared/util/date'
import { authErrorMessage } from '../auth/authError'
import './manual-log.css'

interface SetRow {
  reps: string
  weight: string
}

const blankSet = (): SetRow => ({ reps: '', weight: '' })

/** Default set rows for an exercise: baseSets rows, reps prefilled to the target. */
function defaultSets(ex: ExerciseDto): SetRow[] {
  const reps = (ex.targetRepsHigh ?? ex.targetRepsLow ?? '').toString()
  return Array.from({ length: Math.max(1, ex.baseSets) }, () => ({ reps, weight: '' }))
}

/**
 * "Log Previous Workout" — pick a program day, then record the sets you did.
 * Posts to the SAME sessions endpoint as guided workouts (source=Manual) so it
 * lands in one unified history.
 */
export function ManualLogPage() {
  const navigate = useNavigate()
  const daysQ = useWorkoutDays()
  const createSession = useCreateSession()

  const [date, setDate] = useState(todayIso())
  const [dayId, setDayId] = useState<number | null>(null)
  // Sets keyed by exercise id, seeded from the selected day.
  const [setsByExercise, setSetsByExercise] = useState<Record<number, SetRow[]>>({})
  const [rpe, setRpe] = useState('')
  const [notes, setNotes] = useState('')
  const [error, setError] = useState<string | null>(null)

  const days = daysQ.data ?? []
  const selectedDay = useMemo(() => days.find((d) => d.id === dayId) ?? null, [days, dayId])
  const exercises = useMemo(
    () => (selectedDay ? [...selectedDay.exercises].sort((a, b) => a.sortOrder - b.sortOrder) : []),
    [selectedDay],
  )

  if (daysQ.isLoading) return <LoadingState label="Loading…" />

  const onSelectDay = (value: string) => {
    if (value === '') {
      setDayId(null)
      setSetsByExercise({})
      return
    }
    const id = Number(value)
    const day = days.find((d) => d.id === id)
    setDayId(id)
    setSetsByExercise(
      day ? Object.fromEntries(day.exercises.map((ex) => [ex.id, defaultSets(ex)])) : {},
    )
  }

  const updateSet = (exId: number, idx: number, patch: Partial<SetRow>) =>
    setSetsByExercise((prev) => ({
      ...prev,
      [exId]: (prev[exId] ?? []).map((s, i) => (i === idx ? { ...s, ...patch } : s)),
    }))
  const addSet = (exId: number) =>
    setSetsByExercise((prev) => ({ ...prev, [exId]: [...(prev[exId] ?? []), blankSet()] }))
  const removeSet = (exId: number, idx: number) =>
    setSetsByExercise((prev) => ({ ...prev, [exId]: (prev[exId] ?? []).filter((_, i) => i !== idx) }))

  const totalSets = Object.values(setsByExercise)
    .flat()
    .filter((s) => s.reps.trim() || s.weight.trim()).length

  const submit = async (e: FormEvent) => {
    e.preventDefault()
    setError(null)
    if (!selectedDay) {
      setError('Pick a workout day first.')
      return
    }

    const setLogs: CreateSetLogRequest[] = []
    for (const ex of exercises) {
      ;(setsByExercise[ex.id] ?? []).forEach((s, i) => {
        const reps = s.reps.trim()
        const weight = s.weight.trim()
        if (!reps && !weight) return
        setLogs.push({
          exerciseId: ex.id,
          exerciseName: ex.name,
          setNumber: i + 1,
          repsCompleted: reps ? Number(reps) : null,
          weightKg: weight ? Number(weight) : null,
        })
      })
    }

    if (setLogs.length === 0) {
      setError('Fill in at least one set (reps or weight).')
      return
    }
    if (rpe && (Number(rpe) < 1 || Number(rpe) > 10)) {
      setError('Difficulty (RPE) must be between 1 and 10.')
      return
    }

    const at = new Date(`${date}T12:00:00`).toISOString()
    const body: CreateSessionRequest = {
      workoutDayId: selectedDay.id,
      date,
      startedAt: at,
      completedAt: at,
      source: 'Manual',
      notes: notes.trim() || null,
      perceivedDifficulty: rpe ? Number(rpe) : null,
      setLogs,
    }

    try {
      await createSession.mutateAsync(body)
      navigate('/history')
    } catch (err) {
      setError(authErrorMessage(err, 'Could not save the workout.'))
    }
  }

  return (
    <form className="manual-log" onSubmit={submit}>
      <div className="manual-head">
        <h1>Log a previous workout</h1>
        <p className="muted">Did a session away from the app? Pick the day and record your sets — it joins the same history.</p>
      </div>

      <div className="card card-pad manual-picker">
        <label className="field">
          <span>Workout day</span>
          <select className="manual-day-select" value={dayId ?? ''} onChange={(e) => onSelectDay(e.target.value)} required>
            <option value="" disabled>
              Choose a day…
            </option>
            {days.map((d) => (
              <option key={d.id} value={d.id}>
                {d.name}
              </option>
            ))}
          </select>
        </label>
        <label className="field">
          <span>Date</span>
          <input type="date" value={date} max={todayIso()} onChange={(e) => setDate(e.target.value)} required />
        </label>
      </div>

      {!selectedDay ? (
        <div className="card card-pad">
          <EmptyState title="Pick a workout day">
            Choose one of your program days above to load its exercises, then enter the sets you completed.
          </EmptyState>
        </div>
      ) : (
        <>
          <div className="manual-exercises">
            {exercises.map((ex, i) => {
              const sets = setsByExercise[ex.id] ?? []
              return (
                <div key={ex.id} className="card card-pad manual-exercise">
                  <div className="manual-exercise-head">
                    <span className="manual-ex-index">{i + 1}</span>
                    <div className="manual-ex-title">
                      <span className="manual-ex-name">{ex.name}</span>
                      {ex.primaryMuscles.length > 0 && (
                        <span className="manual-ex-muscles">{ex.primaryMuscles.join(' · ')}</span>
                      )}
                    </div>
                    {ex.repsDisplay && <span className="badge">{ex.repsDisplay}</span>}
                  </div>

                  <div className="manual-sets">
                    <div className="manual-set-head muted">
                      <span>Set</span>
                      <span>Reps</span>
                      <span>Weight (kg)</span>
                      <span />
                    </div>
                    {sets.map((s, idx) => (
                      <div key={idx} className="manual-set-row">
                        <span className="manual-set-num">{idx + 1}</span>
                        <input
                          type="number"
                          min="0"
                          inputMode="numeric"
                          value={s.reps}
                          placeholder="—"
                          onChange={(e) => updateSet(ex.id, idx, { reps: e.target.value })}
                        />
                        <input
                          type="number"
                          min="0"
                          step="0.5"
                          inputMode="decimal"
                          value={s.weight}
                          placeholder="—"
                          onChange={(e) => updateSet(ex.id, idx, { weight: e.target.value })}
                        />
                        {sets.length > 1 ? (
                          <button type="button" className="linklike" title="Remove set" onClick={() => removeSet(ex.id, idx)}>
                            ✕
                          </button>
                        ) : (
                          <span />
                        )}
                      </div>
                    ))}
                    <button type="button" className="btn btn-ghost btn-sm manual-add-set" onClick={() => addSet(ex.id)}>
                      + Add set
                    </button>
                  </div>
                </div>
              )
            })}
          </div>

          <div className="card card-pad manual-meta">
            <label className="field">
              <span>Difficulty / RPE (1-10, optional)</span>
              <input type="number" min="1" max="10" value={rpe} onChange={(e) => setRpe(e.target.value)} />
            </label>
            <label className="field manual-notes">
              <span>Notes (optional)</span>
              <input value={notes} onChange={(e) => setNotes(e.target.value)} placeholder="How did it feel?" />
            </label>
          </div>
        </>
      )}

      {error && <div className="auth-error">{error}</div>}

      <div className="manual-actions">
        <span className="muted manual-count">{totalSets} set{totalSets === 1 ? '' : 's'} to log</span>
        <div className="manual-actions-btns">
          <button type="button" className="btn btn-ghost" onClick={() => navigate('/history')}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary btn-lg" disabled={createSession.isPending || !selectedDay}>
            {createSession.isPending ? 'Saving…' : 'Save workout'}
          </button>
        </div>
      </div>
    </form>
  )
}
