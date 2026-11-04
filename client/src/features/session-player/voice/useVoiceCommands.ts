import { useCallback, useEffect, useRef, useState } from 'react'

/** Intents the workout screen understands, mapped from spoken phrases. */
export type VoiceIntent =
  | 'start'
  | 'pause'
  | 'resume'
  | 'skip'
  | 'next-exercise'
  | 'restart'
  | 'stop'
  | 'how-many-reps'
  | 'how-long-rest'
  | 'what-muscle'

export interface VoiceController {
  /** Whether the browser exposes a speech-recognition API at all. */
  supported: boolean
  /** Whether we are actively listening. */
  listening: boolean
  start: () => void
  stop: () => void
  toggle: () => void
}

interface Options {
  /**
   * Off by default in this phase — the abstraction/wiring exists so a later
   * phase can turn on real recognition without touching the workout logic.
   */
  enabled?: boolean
}

// Minimal typing for the Web Speech API (not in the TS DOM lib by default).
interface SpeechRecognitionLike {
  lang: string
  continuous: boolean
  interimResults: boolean
  start: () => void
  stop: () => void
  onresult: ((event: { results: ArrayLike<ArrayLike<{ transcript: string }>> }) => void) | null
  onend: (() => void) | null
}
type SpeechRecognitionCtor = new () => SpeechRecognitionLike

function getRecognitionCtor(): SpeechRecognitionCtor | null {
  if (typeof window === 'undefined') return null
  const w = window as unknown as {
    SpeechRecognition?: SpeechRecognitionCtor
    webkitSpeechRecognition?: SpeechRecognitionCtor
  }
  return w.SpeechRecognition ?? w.webkitSpeechRecognition ?? null
}

/** Maps a free-text transcript to an intent (null if nothing matches). */
export function matchIntent(transcript: string): VoiceIntent | null {
  const t = transcript.toLowerCase().trim()
  if (/\b(start|begin|go)\b/.test(t)) return 'start'
  if (/\b(pause|hold|wait)\b/.test(t)) return 'pause'
  if (/\b(resume|continue|unpause)\b/.test(t)) return 'resume'
  if (/\bnext exercise\b/.test(t)) return 'next-exercise'
  if (/\b(skip|next)\b/.test(t)) return 'skip'
  if (/\b(restart|repeat|again)\b/.test(t)) return 'restart'
  if (/\b(stop|done|finish|end|i'?m done)\b/.test(t)) return 'stop'
  if (/how many reps/.test(t)) return 'how-many-reps'
  if (/how long.*rest|rest.*how long/.test(t)) return 'how-long-rest'
  if (/what muscle|which muscle/.test(t)) return 'what-muscle'
  return null
}

/**
 * Voice-command seam for the workout player. Calls `onIntent` when a spoken
 * phrase matches. Real recognition is gated behind `enabled` (default false)
 * and degrades to a no-op controller when unsupported, so nothing in the
 * player has to know whether voice is live.
 */
export function useVoiceCommands(
  onIntent: (intent: VoiceIntent) => void,
  options: Options = {},
): VoiceController {
  const ctor = getRecognitionCtor()
  const supported = ctor !== null
  const [listening, setListening] = useState(false)
  const recognitionRef = useRef<SpeechRecognitionLike | null>(null)
  const onIntentRef = useRef(onIntent)
  onIntentRef.current = onIntent

  const enabled = options.enabled ?? false

  const start = useCallback(() => {
    if (!enabled || !ctor) return
    if (recognitionRef.current) return
    const rec = new ctor()
    rec.lang = 'en-US'
    rec.continuous = true
    rec.interimResults = false
    rec.onresult = (event) => {
      const last = event.results[event.results.length - 1]
      const transcript = last?.[0]?.transcript ?? ''
      const intent = matchIntent(transcript)
      if (intent) onIntentRef.current(intent)
    }
    rec.onend = () => setListening(false)
    recognitionRef.current = rec
    rec.start()
    setListening(true)
  }, [ctor, enabled])

  const stop = useCallback(() => {
    recognitionRef.current?.stop()
    recognitionRef.current = null
    setListening(false)
  }, [])

  const toggle = useCallback(() => {
    if (listening) stop()
    else start()
  }, [listening, start, stop])

  // Always release the mic on unmount.
  useEffect(() => stop, [stop])

  return { supported, listening, start, stop, toggle }
}
