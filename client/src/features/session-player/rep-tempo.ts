import type { TempoDto } from '../../shared/api/types'

/**
 * Pure helpers that turn a structured tempo + rep count into a per-rep metronome.
 * A rep runs concentric ("up"/exertion) → optional pause → eccentric
 * ("down"/return). Kept pure and framework-free so it's trivial to reason about.
 */

export type RepPhase = 'concentric' | 'pause' | 'eccentric'

export interface RepTempo {
  concentric: number
  pause: number
  eccentric: number
}

/** Normalises a (possibly partial/null) tempo spec into whole-second phase lengths. */
export function toRepTempo(spec: TempoDto | null | undefined): RepTempo | null {
  if (!spec) return null
  const concentric = Math.max(0, spec.concentricSeconds ?? 0)
  const eccentric = Math.max(0, spec.eccentricSeconds ?? 0)
  const pause = Math.max(0, spec.pauseSeconds ?? 0)
  if (concentric + eccentric + pause <= 0) return null
  return { concentric, pause, eccentric }
}

/** Seconds for one full rep under this tempo. */
export function repCycleSeconds(tempo: RepTempo): number {
  return tempo.concentric + tempo.pause + tempo.eccentric
}

/** Total seconds a tempo-synced work set should run for `reps` reps. */
export function tempoSetSeconds(tempo: RepTempo, reps: number): number {
  return repCycleSeconds(tempo) * Math.max(1, reps)
}

export interface RepPhaseState {
  repNumber: number
  totalReps: number
  phase: RepPhase
  /** Whole seconds remaining in the current phase (ceil). */
  phaseSecondsRemaining: number
  /** 0..1 progress through the current phase. */
  phaseProgress: number
}

/**
 * Given the time already elapsed in a tempo-synced work set, returns which rep and
 * phase the lifter should be in right now.
 */
export function phaseAtElapsed(
  tempo: RepTempo,
  reps: number,
  elapsedSeconds: number,
): RepPhaseState {
  const total = Math.max(1, reps)
  const cycle = repCycleSeconds(tempo)
  const clampedElapsed = Math.min(Math.max(0, elapsedSeconds), cycle * total - 0.0001)

  const repIndex = Math.min(total - 1, Math.floor(clampedElapsed / cycle))
  const within = clampedElapsed - repIndex * cycle

  let phase: RepPhase
  let phaseStart: number
  let phaseLen: number
  if (within < tempo.concentric) {
    phase = 'concentric'
    phaseStart = 0
    phaseLen = tempo.concentric
  } else if (within < tempo.concentric + tempo.pause) {
    phase = 'pause'
    phaseStart = tempo.concentric
    phaseLen = tempo.pause
  } else {
    phase = 'eccentric'
    phaseStart = tempo.concentric + tempo.pause
    phaseLen = tempo.eccentric
  }

  const intoPhase = within - phaseStart
  const safeLen = phaseLen > 0 ? phaseLen : 1
  return {
    repNumber: repIndex + 1,
    totalReps: total,
    phase,
    phaseSecondsRemaining: Math.max(0, Math.ceil(phaseLen - intoPhase)),
    phaseProgress: Math.min(1, intoPhase / safeLen),
  }
}

/** Human label for a phase (the direction of the movement). */
export function phaseLabel(phase: RepPhase): string {
  switch (phase) {
    case 'concentric':
      return 'Lift'
    case 'eccentric':
      return 'Lower'
    case 'pause':
      return 'Hold'
  }
}

/** Up/down arrow for a phase. */
export function phaseArrow(phase: RepPhase): string {
  switch (phase) {
    case 'concentric':
      return '↑'
    case 'eccentric':
      return '↓'
    case 'pause':
      return '•'
  }
}
