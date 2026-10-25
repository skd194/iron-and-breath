import { useMemo } from 'react'
import type { BreathingDto, TempoDto } from '../../../shared/api/types'
import { phaseArrow, phaseAtElapsed, phaseLabel, toRepTempo } from '../rep-tempo'

interface RepTempoGuideProps {
  tempoSpec?: TempoDto | null
  reps?: number | null
  breathing?: BreathingDto | null
  /** Total length of this work set in seconds. */
  totalSeconds: number
  /** Live remaining time from the session runner. */
  remainingMs: number
}

/**
 * The per-rep metronome shown during a work set: calls the current phase
 * (Lift ↑ / Lower ↓ / Hold), counts down the phase, tells the lifter to
 * inhale/exhale, and tracks the rep count — all derived from the exercise's
 * structured tempo and the runner's countdown (no extra timer). Renders nothing
 * when the exercise has no structured tempo (the caller shows a breathing cue).
 */
export function RepTempoGuide({ tempoSpec, reps, breathing, totalSeconds, remainingMs }: RepTempoGuideProps) {
  const tempo = useMemo(() => toRepTempo(tempoSpec), [tempoSpec])

  if (!tempo || !reps || reps <= 0) return null

  const elapsed = totalSeconds - remainingMs / 1000
  const state = phaseAtElapsed(tempo, reps, elapsed)

  const breathWord =
    state.phase === 'concentric'
      ? breathing?.concentric ?? 'Exhale'
      : state.phase === 'eccentric'
        ? breathing?.eccentric ?? 'Inhale'
        : 'Hold'

  return (
    <div className={`rep-tempo rep-tempo-${state.phase}`} aria-live="polite">
      <div className="rep-tempo-count mono">
        Rep {state.repNumber}
        <span className="rep-tempo-total">/ {state.totalReps}</span>
      </div>

      <div className="rep-tempo-phase">
        <span className="rep-tempo-arrow" aria-hidden>
          {phaseArrow(state.phase)}
        </span>
        <span className="rep-tempo-word">{phaseLabel(state.phase)}</span>
        <span className="rep-tempo-secs mono">{state.phaseSecondsRemaining}s</span>
      </div>

      <div className="rep-tempo-bar" aria-hidden>
        <span className="rep-tempo-bar-fill" style={{ width: `${state.phaseProgress * 100}%` }} />
      </div>

      <div className="rep-tempo-breath">🫁 {breathWord}</div>
    </div>
  )
}
