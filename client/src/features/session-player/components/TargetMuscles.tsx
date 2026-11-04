import type { MuscleDto } from '../../../shared/api/types'

interface TargetMusclesProps {
  primary?: string[]
  secondary?: string[]
  /** Smaller chips for the rest screen / tight spaces. */
  compact?: boolean
  /** Looks up a muscle's catalog entry so the chip tooltip can explain it. */
  lookup?: (name: string) => MuscleDto | undefined
}

/**
 * Primary/secondary target-muscle chips. Green = primary (worked hardest), grey =
 * secondary (assists). Each chip's tooltip explains the role and what the muscle
 * does. Renders nothing when no muscles are configured.
 */
export function TargetMuscles({ primary = [], secondary = [], compact = false, lookup }: TargetMusclesProps) {
  if (primary.length === 0 && secondary.length === 0) return null

  const titleFor = (name: string, role: 'primary' | 'secondary') => {
    const roleText =
      role === 'primary'
        ? 'Primary muscle — worked hardest by this exercise.'
        : 'Secondary muscle — assists the movement.'
    const fn = lookup?.(name)?.function
    return fn ? `${name} · ${roleText} ${fn}` : `${name} · ${roleText}`
  }

  return (
    <div className={`muscles${compact ? ' compact' : ''}`} aria-label="Target muscles">
      {primary.map((m) => (
        <span key={`p-${m}`} className="muscle-chip primary" title={titleFor(m, 'primary')}>
          {m}
        </span>
      ))}
      {secondary.map((m) => (
        <span key={`s-${m}`} className="muscle-chip secondary" title={titleFor(m, 'secondary')}>
          {m}
        </span>
      ))}
      {secondary.length > 0 && (
        <span className="muscles-legend" title="Green = primary muscle (worked hardest). Grey = secondary (assists). Hover a muscle to see what it does.">
          <i className="legend-dot primary" aria-hidden /> primary
          <i className="legend-dot secondary" aria-hidden /> assists
        </span>
      )}
    </div>
  )
}
