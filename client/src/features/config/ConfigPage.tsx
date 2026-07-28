import { useEffect, useState, type FormEvent } from 'react'
import {
  useCreateDay,
  useDeleteDay,
  useDeleteExercise,
  useReorderDays,
  useReorderExercises,
  useSettings,
  useUpdateDay,
  useUpdateSettings,
  useVideoLibrary,
  useWorkoutDays,
} from '../../shared/api/hooks'
import type { ExerciseDto, VideoLibraryItemDto, WorkoutDayDto } from '../../shared/api/types'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import { ExerciseModal } from './ExerciseModal'
import './config.css'

export function ConfigPage() {
  const daysQ = useWorkoutDays()
  const videosQ = useVideoLibrary()

  return (
    <div className="config grid">
      <div>
        <h1 className="page-title">Configure</h1>
        <p className="muted">Tune your program, timings, and every exercise. Changes are yours alone.</p>
      </div>

      <SettingsSection />

      <section className="grid">
        <div className="config-section-head">
          <div className="section-title" style={{ margin: 0 }}>
            Your program
          </div>
          <AddDayButton />
        </div>

        {daysQ.isLoading ? (
          <LoadingState label="Loading your program…" />
        ) : daysQ.isError || !daysQ.data ? (
          <ErrorState message="Couldn't load your program." onRetry={() => daysQ.refetch()} />
        ) : (
          daysQ.data.map((day, i) => (
            <DayCard
              key={day.id}
              day={day}
              index={i}
              total={daysQ.data!.length}
              allDayIds={daysQ.data!.map((d) => d.id)}
              videos={videosQ.data ?? []}
            />
          ))
        )}
      </section>
    </div>
  )
}

function SettingsSection() {
  const settingsQ = useSettings()
  const update = useUpdateSettings()
  const [startDate, setStartDate] = useState('')
  const [target, setTarget] = useState(4)
  const [saved, setSaved] = useState(false)

  useEffect(() => {
    if (settingsQ.data) {
      setStartDate(settingsQ.data.programStartDate)
      setTarget(settingsQ.data.daysPerWeekTarget)
    }
  }, [settingsQ.data])

  const save = (e: FormEvent) => {
    e.preventDefault()
    setSaved(false)
    update.mutate(
      { programStartDate: startDate, daysPerWeekTarget: target },
      { onSuccess: () => setSaved(true) },
    )
  }

  return (
    <section className="card card-pad">
      <div className="section-title">Program settings</div>
      <form className="config-settings" onSubmit={save}>
        <label className="field">
          Program start date
          <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
        </label>
        <label className="field">
          Weekly target (days)
          <input
            type="number"
            min="1"
            max="7"
            value={target}
            onChange={(e) => setTarget(Number(e.target.value))}
            required
          />
        </label>
        <div className="config-settings-actions">
          <button className="btn btn-primary" type="submit" disabled={update.isPending}>
            {update.isPending ? 'Saving…' : 'Save settings'}
          </button>
          {saved && <span className="badge ok">Saved</span>}
        </div>
      </form>
    </section>
  )
}

function AddDayButton() {
  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')
  const [focus, setFocus] = useState('')
  const create = useCreateDay()

  const submit = (e: FormEvent) => {
    e.preventDefault()
    if (!name.trim()) return
    create.mutate(
      { name: name.trim(), focus: focus.trim() || undefined },
      {
        onSuccess: () => {
          setName('')
          setFocus('')
          setOpen(false)
        },
      },
    )
  }

  if (!open) {
    return (
      <button className="btn btn-primary btn-sm" onClick={() => setOpen(true)}>
        + Add day
      </button>
    )
  }

  return (
    <div className="modal-overlay" onClick={() => setOpen(false)}>
      <form className="card card-pad modal" onClick={(e) => e.stopPropagation()} onSubmit={submit}>
        <h3>Add a workout day</h3>
        <label className="field">
          Name
          <input value={name} onChange={(e) => setName(e.target.value)} required autoFocus />
        </label>
        <label className="field">
          Focus <span className="muted">(optional)</span>
          <input value={focus} onChange={(e) => setFocus(e.target.value)} />
        </label>
        <div className="modal-actions">
          <button type="button" className="btn btn-ghost" onClick={() => setOpen(false)}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={create.isPending}>
            {create.isPending ? 'Adding…' : 'Add day'}
          </button>
        </div>
      </form>
    </div>
  )
}

function DayCard({
  day,
  index,
  total,
  allDayIds,
  videos,
}: {
  day: WorkoutDayDto
  index: number
  total: number
  allDayIds: number[]
  videos: VideoLibraryItemDto[]
}) {
  const [editing, setEditing] = useState(false)
  const [name, setName] = useState(day.name)
  const [focus, setFocus] = useState(day.focus)
  const [modal, setModal] = useState<{ exercise?: ExerciseDto } | null>(null)

  const updateDay = useUpdateDay()
  const deleteDay = useDeleteDay()
  const reorderDays = useReorderDays()
  const deleteExercise = useDeleteExercise()
  const reorderExercises = useReorderExercises()

  const saveDay = () => {
    updateDay.mutate(
      { id: day.id, body: { name: name.trim() || day.name, focus: focus.trim() } },
      { onSuccess: () => setEditing(false) },
    )
  }

  const removeDay = () => {
    if (confirm(`Delete "${day.name}" and all its exercises?`)) {
      deleteDay.mutate(day.id, {
        onError: (err) => alert(err instanceof Error ? err.message : 'Could not delete this day.'),
      })
    }
  }

  const moveDay = (dir: -1 | 1) => {
    const ids = [...allDayIds]
    const j = index + dir
    if (j < 0 || j >= ids.length) return
    ;[ids[index], ids[j]] = [ids[j], ids[index]]
    reorderDays.mutate(ids)
  }

  const moveExercise = (exIndex: number, dir: -1 | 1) => {
    const ids = day.exercises.map((e) => e.id)
    const j = exIndex + dir
    if (j < 0 || j >= ids.length) return
    ;[ids[exIndex], ids[j]] = [ids[j], ids[exIndex]]
    reorderExercises.mutate({ dayId: day.id, orderedIds: ids })
  }

  const removeExercise = (ex: ExerciseDto) => {
    if (confirm(`Remove "${ex.name}"?`)) {
      deleteExercise.mutate({ dayId: day.id, exerciseId: ex.id })
    }
  }

  return (
    <div className="card card-pad config-day">
      <div className="config-day-head">
        {editing ? (
          <div className="config-day-edit">
            <input className="config-inline-input" value={name} onChange={(e) => setName(e.target.value)} />
            <input
              className="config-inline-input"
              placeholder="Focus"
              value={focus}
              onChange={(e) => setFocus(e.target.value)}
            />
            <div className="config-inline-actions">
              <button className="btn btn-primary btn-sm" onClick={saveDay} disabled={updateDay.isPending}>
                Save
              </button>
              <button className="btn btn-ghost btn-sm" onClick={() => setEditing(false)}>
                Cancel
              </button>
            </div>
          </div>
        ) : (
          <>
            <div>
              <h3 className="config-day-name">{day.name}</h3>
              {day.focus && <div className="muted config-day-focus">{day.focus}</div>}
            </div>
            <div className="config-day-tools">
              <button className="icon-btn" title="Move up" onClick={() => moveDay(-1)} disabled={index === 0}>
                ↑
              </button>
              <button
                className="icon-btn"
                title="Move down"
                onClick={() => moveDay(1)}
                disabled={index === total - 1}
              >
                ↓
              </button>
              <button className="btn btn-ghost btn-sm" onClick={() => setEditing(true)}>
                Edit
              </button>
              <button className="btn btn-danger btn-sm" onClick={removeDay} disabled={deleteDay.isPending}>
                Delete
              </button>
            </div>
          </>
        )}
      </div>

      <ul className="config-exercises">
        {day.exercises.map((ex, i) => (
          <li key={ex.id} className="config-exercise">
            <div className="config-exercise-main">
              <span className="config-exercise-name">{ex.name}</span>
              <span className="muted config-exercise-meta">
                {ex.baseSets} sets{ex.repsDisplay ? ` · ${ex.repsDisplay}` : ''}
                {ex.video ? ' · 🎬' : ''}
              </span>
            </div>
            <div className="config-exercise-tools">
              <button className="icon-btn" title="Up" onClick={() => moveExercise(i, -1)} disabled={i === 0}>
                ↑
              </button>
              <button
                className="icon-btn"
                title="Down"
                onClick={() => moveExercise(i, 1)}
                disabled={i === day.exercises.length - 1}
              >
                ↓
              </button>
              <button className="btn btn-ghost btn-sm" onClick={() => setModal({ exercise: ex })}>
                Edit
              </button>
              <button className="btn btn-danger btn-sm" onClick={() => removeExercise(ex)}>
                ✕
              </button>
            </div>
          </li>
        ))}
      </ul>

      <button className="btn btn-ghost btn-sm config-add-ex" onClick={() => setModal({})}>
        + Add exercise
      </button>

      {modal && (
        <ExerciseModal
          dayId={day.id}
          exercise={modal.exercise}
          videos={videos}
          onClose={() => setModal(null)}
        />
      )}
    </div>
  )
}
