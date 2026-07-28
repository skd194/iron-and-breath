import { useState, type FormEvent } from 'react'
import { useCreateExercise, useUpdateExercise } from '../../shared/api/hooks'
import type { ExerciseDto, UpsertExerciseRequest, VideoLibraryItemDto } from '../../shared/api/types'
import { authErrorMessage } from '../auth/authError'

interface Props {
  dayId: number
  exercise?: ExerciseDto
  videos: VideoLibraryItemDto[]
  onClose: () => void
}

/** Add or edit a single exercise. Reused for both via the optional `exercise`. */
export function ExerciseModal({ dayId, exercise, videos, onClose }: Props) {
  const isEdit = !!exercise
  const create = useCreateExercise()
  const update = useUpdateExercise()

  const [name, setName] = useState(exercise?.name ?? '')
  const [repsDisplay, setRepsDisplay] = useState(exercise?.repsDisplay ?? '')
  const [low, setLow] = useState<string>(exercise?.targetRepsLow?.toString() ?? '')
  const [high, setHigh] = useState<string>(exercise?.targetRepsHigh?.toString() ?? '')
  const [baseSets, setBaseSets] = useState<string>(exercise?.baseSets?.toString() ?? '3')
  const [cue, setCue] = useState(exercise?.cue ?? '')
  const [videoId, setVideoId] = useState<string>(
    exercise?.video ? matchVideoId(exercise, videos)?.toString() ?? '' : '',
  )
  const [error, setError] = useState<string | null>(null)

  const busy = create.isPending || update.isPending

  const submit = async (e: FormEvent) => {
    e.preventDefault()
    setError(null)
    const body: UpsertExerciseRequest = {
      name: name.trim(),
      repsDisplay: repsDisplay.trim() || undefined,
      targetRepsLow: low ? Number(low) : null,
      targetRepsHigh: high ? Number(high) : null,
      baseSets: Number(baseSets) || 1,
      cue: cue.trim() || null,
      videoId: videoId ? Number(videoId) : null,
    }
    try {
      if (isEdit && exercise) {
        await update.mutateAsync({ dayId, exerciseId: exercise.id, body })
      } else {
        await create.mutateAsync({ dayId, body })
      }
      onClose()
    } catch (err) {
      setError(authErrorMessage(err, 'Could not save the exercise.'))
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <form className="card card-pad modal" onClick={(e) => e.stopPropagation()} onSubmit={submit}>
        <h3>{isEdit ? 'Edit exercise' : 'Add exercise'}</h3>

        <label className="field">
          Name
          <input value={name} onChange={(e) => setName(e.target.value)} required autoFocus />
        </label>

        <label className="field">
          Target text <span className="muted">(e.g. "3 sets x 10-12 reps")</span>
          <input value={repsDisplay} onChange={(e) => setRepsDisplay(e.target.value)} />
        </label>

        <div className="config-row-3">
          <label className="field">
            Reps low
            <input type="number" min="0" value={low} onChange={(e) => setLow(e.target.value)} />
          </label>
          <label className="field">
            Reps high
            <input type="number" min="0" value={high} onChange={(e) => setHigh(e.target.value)} />
          </label>
          <label className="field">
            Base sets
            <input
              type="number"
              min="1"
              max="12"
              value={baseSets}
              onChange={(e) => setBaseSets(e.target.value)}
              required
            />
          </label>
        </div>

        <label className="field">
          Cue
          <input value={cue} onChange={(e) => setCue(e.target.value)} />
        </label>

        <label className="field">
          Video
          <select value={videoId} onChange={(e) => setVideoId(e.target.value)}>
            <option value="">No video</option>
            {videos.map((v) => (
              <option key={v.id} value={v.id}>
                {v.title}
              </option>
            ))}
          </select>
        </label>

        {error && <div className="auth-error">{error}</div>}

        <div className="modal-actions">
          <button type="button" className="btn btn-ghost" onClick={onClose} disabled={busy}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={busy}>
            {busy ? 'Saving…' : 'Save'}
          </button>
        </div>
      </form>
    </div>
  )
}

/** The exercise DTO carries a resolved VideoDto, not its id. Match by embed URL. */
function matchVideoId(exercise: ExerciseDto, videos: VideoLibraryItemDto[]): number | undefined {
  if (!exercise.video) return undefined
  return videos.find((v) => v.video.embedUrl === exercise.video!.embedUrl)?.id
}
