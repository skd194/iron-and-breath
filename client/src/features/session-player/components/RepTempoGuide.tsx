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
 * The per-rep metronome shown during a work set: a big breathing orb that grows on
 * the inhale and shrinks on the exhale (synced to the rep), the phase call
 * (Lift ↑ / Lower ↓ / Hold) with a per-phase countdown, and the rep count — all
 * derived from the exercise's structured tempo and the runner's countdown.
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

  // The orb grows while inhaling (eccentric/lowering), shrinks while exhaling
  // (concentric/lifting), and holds at mid-size on a pause.
  const scale =
    state.phase === 'eccentric'
      ? 0.62 + 0.38 * state.phaseProgress
      : state.phase === 'concentric'
        ? 1.0 - 0.38 * state.phaseProgress
        : 0.8

  return (
    <div className={`rep-tempo rep-tempo-${state.phase}`} aria-live="polite">
      <div className="rep-tempo-count mono">
        Rep {state.repNumber}
        <span className="rep-tempo-total">/ {state.totalReps}</span>
      </div>

      {/* Big animated breathing orb */}
      <div className="rep-breath">
        <div className="rep-breath-orb" style={{ transform: `scale(${scale.toFixed(3)})` }}>
          <span className="rep-breath-ring" />
        </div>
        <div className="rep-breath-word">
          🫁 {breathWord}
        </div>
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
    </div>
  )
}
