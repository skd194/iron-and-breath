import type { VideoDto } from '../../../shared/api/types'
import { VideoEmbed } from './VideoEmbed'

interface ExerciseAnimationProps {
  animationRef?: string | null
  video?: VideoDto | null
  name: string
  /** Smaller footprint for the work screen so it doesn't bury the coaching. */
  compact?: boolean
}

/**
 * Pluggable movement-demonstration slot. Resolves an `animationRef` to a real
 * asset when one is registered; otherwise falls back to the instructional video,
 * then to a styled placeholder. Keeping asset resolution here (not in the
 * workout logic) means real animations can be dropped in later by registering
 * them in ANIMATION_ASSETS — nothing else changes.
 */
export function ExerciseAnimation({ animationRef, video, name, compact = false }: ExerciseAnimationProps) {
  const asset = resolveAnimation(animationRef)
  const cls = `exercise-animation${compact ? ' compact' : ''}`

  if (asset) {
    return (
      <div className={cls}>
        <img src={asset} alt={`${name} movement demonstration`} />
      </div>
    )
  }

  if (video?.embedUrl) {
    return (
      <div className={compact ? 'exercise-animation-wrap compact' : 'exercise-animation-wrap'}>
        <VideoEmbed video={video} />
      </div>
    )
  }

  return (
    <div className={`${cls} placeholder`}>
      <span className="anim-emoji" aria-hidden>🏋️</span>
      <span className="muted">Animation coming soon</span>
    </div>
  )
}

// Asset registry: maps an exercise's `animationRef` key to a media URL. Empty
// for now — real animation assets get registered here in a later phase.
const ANIMATION_ASSETS: Record<string, string> = {}

function resolveAnimation(ref?: string | null): string | null {
  if (!ref) return null
  return ANIMATION_ASSETS[ref] ?? null
}
