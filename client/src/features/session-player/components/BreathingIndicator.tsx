import type { BreathingDto } from '../../../shared/api/types'

interface BreathingIndicatorProps {
  breathing?: BreathingDto | null
}

/**
 * Visual breathing guidance driven entirely by exercise metadata (never
 * hard-coded per screen). Shows the exhale (exertion) / inhale (return) cues,
 * or a steady-breathing note for timed holds. Renders nothing when unset.
 */
export function BreathingIndicator({ breathing }: BreathingIndicatorProps) {
  if (!breathing) return null
  const { concentric, eccentric, notes } = breathing

  // Holds and other edge cases only carry a note.
  if (!concentric && !eccentric) {
    return notes ? <div className="breathing breathing-note">🫁 {notes}</div> : null
  }

  return (
    <div className="breathing" aria-label="Breathing guidance">
      <div className="breathing-phase">
        <span className="breathing-arrow up" aria-hidden>↑</span>
        <span className="breathing-word">{concentric ?? '—'}</span>
        <span className="breathing-cap">exertion</span>
      </div>
      <div className="breathing-divider" aria-hidden />
      <div className="breathing-phase">
        <span className="breathing-arrow down" aria-hidden>↓</span>
        <span className="breathing-word">{eccentric ?? '—'}</span>
        <span className="breathing-cap">return</span>
      </div>
    </div>
  )
}
