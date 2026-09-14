interface TargetMusclesProps {
  primary?: string[]
  secondary?: string[]
  /** Smaller chips for the rest screen / tight spaces. */
  compact?: boolean
}

/**
 * Primary/secondary target-muscle chips. Foundation for a visual muscle diagram
 * in a later phase; renders nothing when no muscles are configured.
 */
export function TargetMuscles({ primary = [], secondary = [], compact = false }: TargetMusclesProps) {
  if (primary.length === 0 && secondary.length === 0) return null
  return (
    <div className={`muscles${compact ? ' compact' : ''}`} aria-label="Target muscles">
      {primary.map((m) => (
        <span key={`p-${m}`} className="muscle-chip primary">{m}</span>
      ))}
      {secondary.map((m) => (
        <span key={`s-${m}`} className="muscle-chip secondary">{m}</span>
      ))}
    </div>
  )
}
