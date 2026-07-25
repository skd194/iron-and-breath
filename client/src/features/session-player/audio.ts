// Tiny Web Audio cue generator — avoids shipping audio files. A short blip on
// each step transition and a two-tone chime on completion.
let ctx: AudioContext | null = null

function getCtx(): AudioContext | null {
  if (typeof window === 'undefined') return null
  if (!ctx) {
    const Ctor = window.AudioContext ?? (window as unknown as { webkitAudioContext?: typeof AudioContext }).webkitAudioContext
    if (!Ctor) return null
    ctx = new Ctor()
  }
  return ctx
}

function tone(freq: number, startOffset: number, durationMs: number) {
  const audio = getCtx()
  if (!audio) return
  const osc = audio.createOscillator()
  const gain = audio.createGain()
  osc.type = 'sine'
  osc.frequency.value = freq
  const t0 = audio.currentTime + startOffset
  gain.gain.setValueAtTime(0.0001, t0)
  gain.gain.exponentialRampToValueAtTime(0.25, t0 + 0.01)
  gain.gain.exponentialRampToValueAtTime(0.0001, t0 + durationMs / 1000)
  osc.connect(gain).connect(audio.destination)
  osc.start(t0)
  osc.stop(t0 + durationMs / 1000 + 0.02)
}

/** A single transition blip. */
export function cueTransition() {
  tone(660, 0, 160)
}

/** A rising two-tone chime for session completion. */
export function cueComplete() {
  tone(660, 0, 180)
  tone(880, 0.18, 260)
}

/** Some browsers require a user gesture to unlock audio. */
export function unlockAudio() {
  const audio = getCtx()
  if (audio && audio.state === 'suspended') {
    void audio.resume()
  }
}
