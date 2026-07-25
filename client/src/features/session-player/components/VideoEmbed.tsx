import { useState } from 'react'
import type { VideoDto } from '../../../shared/api/types'

interface VideoEmbedProps {
  video: VideoDto | null | undefined
}

/**
 * Passive form-reference video. Loaded only when its step is active (this
 * component is unmounted otherwise), muted, and never auto-advances the timer —
 * the timer is the source of truth for pacing.
 */
export function VideoEmbed({ video }: VideoEmbedProps) {
  const [playing, setPlaying] = useState(false)

  if (!video || !video.embedUrl) {
    return (
      <div className="video-embed placeholder">
        <span className="muted">No form video yet</span>
        <small className="muted">Add one via the seed script (milestone 5)</small>
      </div>
    )
  }

  // Only build the iframe src once the user opts to play, so nothing loads or
  // makes sound until requested. Muted + no autoplay by default.
  const src = `${video.embedUrl}?rel=0&modestbranding=1&playsinline=1&mute=1${playing ? '&autoplay=1' : ''}`

  return (
    <div className="video-embed">
      {playing ? (
        <iframe
          src={src}
          title={video.title}
          allow="accelerometer; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen
          loading="lazy"
        />
      ) : (
        <button className="video-poster" onClick={() => setPlaying(true)} aria-label="Play form video">
          {video.thumbnailUrl ? (
            <img src={video.thumbnailUrl} alt={video.title} />
          ) : (
            <div className="video-poster-fallback" />
          )}
          <span className="video-play">▶</span>
          <span className="video-title">{video.title}</span>
        </button>
      )}
    </div>
  )
}
