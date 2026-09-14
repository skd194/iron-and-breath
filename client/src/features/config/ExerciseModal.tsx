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

  // Coaching metadata (drives the interactive workout + rest screens).
  const [breathIn, setBreathIn] = useState(exercise?.breathing?.concentric ?? '')
  const [breathOut, setBreathOut] = useState(exercise?.breathing?.eccentric ?? '')
  const [breathNotes, setBreathNotes] = useState(exercise?.breathing?.notes ?? '')
  const [primaryMuscles, setPrimaryMuscles] = useState((exercise?.primaryMuscles ?? []).join(', '))
  const [secondaryMuscles, setSecondaryMuscles] = useState((exercise?.secondaryMuscles ?? []).join(', '))
  const [tempo, setTempo] = useState(exercise?.tempo ?? '')
  const [benefits, setBenefits] = useState(exercise?.benefits ?? '')
  const [commonMistakes, setCommonMistakes] = useState(exercise?.commonMistakes ?? '')
  const [safetyTips, setSafetyTips] = useState(exercise?.safetyTips ?? '')

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
      breathingConcentric: breathIn.trim() || null,
      breathingEccentric: breathOut.trim() || null,
      breathingNotes: breathNotes.trim() || null,
      primaryMuscles: splitMuscles(primaryMuscles),
      secondaryMuscles: splitMuscles(secondaryMuscles),
      tempo: tempo.trim() || null,
      benefits: benefits.trim() || null,
      commonMistakes: commonMistakes.trim() || null,
      safetyTips: safetyTips.trim() || null,
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

        <details className="config-details">
          <summary>Coaching details (breathing, muscles, tips)</summary>

          <div className="config-row-3">
            <label className="field">
              Breathe out (exertion)
              <input value={breathIn} onChange={(e) => setBreathIn(e.target.value)} placeholder="Exhale" />
            </label>
            <label className="field">
              Breathe in (return)
              <input value={breathOut} onChange={(e) => setBreathOut(e.target.value)} placeholder="Inhale" />
            </label>
            <label className="field">
              Breathing note
              <input value={breathNotes} onChange={(e) => setBreathNotes(e.target.value)} placeholder="For holds" />
            </label>
          </div>

          <label className="field">
            Primary muscles <span className="muted">(comma-separated)</span>
            <input value={primaryMuscles} onChange={(e) => setPrimaryMuscles(e.target.value)} placeholder="Chest, Triceps" />
          </label>
          <label className="field">
            Secondary muscles <span className="muted">(comma-separated)</span>
            <input value={secondaryMuscles} onChange={(e) => setSecondaryMuscles(e.target.value)} placeholder="Shoulders" />
          </label>

          <label className="field">
            Tempo
            <input value={tempo} onChange={(e) => setTempo(e.target.value)} placeholder="2-0-2" />
          </label>
          <label className="field">
            Benefits
            <input value={benefits} onChange={(e) => setBenefits(e.target.value)} />
          </label>
          <label className="field">
            Common mistakes
            <input value={commonMistakes} onChange={(e) => setCommonMistakes(e.target.value)} />
          </label>
          <label className="field">
            Safety tips
            <input value={safetyTips} onChange={(e) => setSafetyTips(e.target.value)} />
          </label>
        </details>

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

/** Splits a comma-separated muscle input into a clean array. */
function splitMuscles(value: string): string[] {
  return value
    .split(',')
    .map((m) => m.trim())
    .filter(Boolean)
}
