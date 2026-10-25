import { useEffect, useState } from 'react'
import type { VideoDto } from '../../../shared/api/types'
import { framesFor } from './exercise-frames'
import { VideoEmbed } from './VideoEmbed'

interface ExerciseAnimationProps {
  animationRef?: string | null
  video?: VideoDto | null
  name: string
  /** Smaller footprint for the work screen so it doesn't bury the coaching. */
  compact?: boolean
}

/**
 * Movement-demonstration slot. Prefers an animated two-frame clip from the
 * exercise-media library (free-exercise-db, CC0); falls back to the instructional
 * video, then to a one-tap "Watch form on YouTube" search for anything uncovered
 * (yoga, skipping, warm-ups).
 */
export function ExerciseAnimation({ animationRef, video, name, compact = false }: ExerciseAnimationProps) {
  const frames = framesFor(animationRef)
  const cls = `exercise-animation${compact ? ' compact' : ''}`
  const ytUrl = `https://www.youtube.com/results?search_query=${encodeURIComponent(`${name} proper form technique`)}`

  // Animated demo (if we have frames) or the curated clip, plus a YouTube link
  // so the user always has a full video to watch too.
  if (frames) {
    return (
      <div className="exercise-demo">
        <AnimatedFrames frames={frames} name={name} className={cls} />
        <a className="watch-yt" href={ytUrl} target="_blank" rel="noopener noreferrer">
          ▶ Watch form on YouTube
        </a>
      </div>
    )
  }

  if (video?.embedUrl) {
    return (
      <div className="exercise-demo">
        <div className={compact ? 'exercise-animation-wrap compact' : 'exercise-animation-wrap'}>
          <VideoEmbed video={video} />
        </div>
        <a className="watch-yt" href={ytUrl} target="_blank" rel="noopener noreferrer">
          ▶ More on YouTube
        </a>
      </div>
    )
  }

  return (
    <a
      className={`${cls} placeholder video-search`}
      href={ytUrl}
      target="_blank"
      rel="noopener noreferrer"
    >
      <span className="anim-emoji" aria-hidden>▶</span>
      <span className="video-search-label">Watch form on YouTube</span>
      <span className="muted video-search-sub">{name}</span>
    </a>
  )
}

/** Cross-fades between the start/end frames to animate the movement. */
function AnimatedFrames({ frames, name, className }: { frames: string[]; name: string; className: string }) {
  const [i, setI] = useState(0)

  useEffect(() => {
    if (frames.length < 2) return
    const id = window.setInterval(() => setI((p) => (p + 1) % frames.length), 1100)
    return () => window.clearInterval(id)
  }, [frames])

  return (
    <div className={`${className} frames`}>
      {frames.map((src, idx) => (
        <img
          key={src}
          src={src}
          alt={`${name} — movement frame ${idx + 1}`}
          loading="lazy"
          className={idx === i ? 'on' : ''}
        />
      ))}
    </div>
  )
}
