import { useCallback, useEffect, useRef, useState } from 'react'
import type { SessionStep } from './session-plan'

export type RunnerStatus = 'running' | 'paused' | 'complete'

export interface SessionRunner {
  index: number
  step: SessionStep | undefined
  remainingMs: number
  status: RunnerStatus
  totalSteps: number
  pause: () => void
  resume: () => void
  togglePause: () => void
  skip: () => void
  skipExercise: () => void
  restartStep: () => void
}

interface Options {
  onComplete?: () => void
  /** Fired when a new step becomes active (including the first). Good for audio cues. */
  onStepStart?: (step: SessionStep, index: number) => void
  /**
   * When set, the current step index is mirrored to sessionStorage under this
   * key so that navigating away and back resumes the session instead of losing
   * it. Cleared automatically on completion.
   */
  persistKey?: string
}

/** Reads a previously-persisted step index, clamped to the current plan. */
function readPersistedIndex(key: string | undefined, stepCount: number): number {
  if (!key) return 0
  try {
    const raw = sessionStorage.getItem(key)
    if (!raw) return 0
    const n = (JSON.parse(raw) as { index?: unknown }).index
    return typeof n === 'number' && n >= 0 && n < stepCount ? n : 0
  } catch {
    return 0
  }
}

/**
 * Drives the scripted session: a monotonic countdown per step that auto-advances
 * at zero. The timer is the source of truth for pacing. Uses a wall-clock
 * deadline so it stays accurate regardless of tick jitter, and refs to avoid
 * stale closures inside the interval.
 */
export function useSessionRunner(steps: SessionStep[], options: Options = {}): SessionRunner {
  const [index, setIndex] = useState(() => readPersistedIndex(options.persistKey, steps.length))
  const [status, setStatus] = useState<RunnerStatus>('running')
  const [remainingMs, setRemainingMs] = useState(() => ((steps[index] ?? steps[0])?.durationSeconds ?? 0) * 1000)

  const deadlineRef = useRef<number>(0)
  const remainingRef = useRef<number>(remainingMs)
  const indexRef = useRef(0)
  const statusRef = useRef<RunnerStatus>('running')

  const onCompleteRef = useRef(options.onComplete)
  const onStepStartRef = useRef(options.onStepStart)
  const persistKeyRef = useRef(options.persistKey)
  onCompleteRef.current = options.onComplete
  onStepStartRef.current = options.onStepStart
  persistKeyRef.current = options.persistKey

  indexRef.current = index
  statusRef.current = status
  remainingRef.current = remainingMs

  // Mirror progress to sessionStorage so leaving the screen and coming back
  // resumes at the same step rather than losing the session.
  useEffect(() => {
    const key = persistKeyRef.current
    if (!key) return
    try {
      sessionStorage.setItem(key, JSON.stringify({ index }))
    } catch {
      /* storage unavailable — non-fatal */
    }
  }, [index])

  // Initialise a step whenever the index changes (and on first mount).
  useEffect(() => {
    const step = steps[index]
    if (!step) return
    const ms = step.durationSeconds * 1000
    remainingRef.current = ms
    setRemainingMs(ms)
    deadlineRef.current = performance.now() + ms
    setStatus('running')
    onStepStartRef.current?.(step, index)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [index, steps])

  const complete = useCallback(() => {
    setStatus('complete')
    const key = persistKeyRef.current
    if (key) {
      try {
        sessionStorage.removeItem(key)
      } catch {
        /* non-fatal */
      }
    }
    onCompleteRef.current?.()
  }, [])

  const advance = useCallback(() => {
    const i = indexRef.current
    if (i < steps.length - 1) {
      setIndex(i + 1)
    } else {
      complete()
    }
  }, [steps.length, complete])

  const advanceRef = useRef(advance)
  advanceRef.current = advance

  // Single ticking interval for the lifetime of the runner.
  useEffect(() => {
    const id = window.setInterval(() => {
      if (statusRef.current !== 'running') return
      const rem = deadlineRef.current - performance.now()
      if (rem <= 0) {
        advanceRef.current()
      } else {
        setRemainingMs(rem)
      }
    }, 100)
    return () => window.clearInterval(id)
  }, [])

  const pause = useCallback(() => {
    if (statusRef.current !== 'running') return
    const rem = Math.max(0, deadlineRef.current - performance.now())
    remainingRef.current = rem
    setRemainingMs(rem)
    setStatus('paused')
  }, [])

  const resume = useCallback(() => {
    if (statusRef.current !== 'paused') return
    deadlineRef.current = performance.now() + remainingRef.current
    setStatus('running')
  }, [])

  const togglePause = useCallback(() => {
    if (statusRef.current === 'running') pause()
    else if (statusRef.current === 'paused') resume()
  }, [pause, resume])

  const skip = useCallback(() => {
    advanceRef.current()
  }, [])

  // Jump past every remaining step belonging to the current exercise (its other
  // sets, inter-set rests, and the trailing inter-exercise rest) to the next
  // exercise's first work step.
  const skipExercise = useCallback(() => {
    const i = indexRef.current
    const currentName = steps[i]?.exerciseName
    let j = i + 1
    while (j < steps.length) {
      const s = steps[j]
      if (s.kind === 'work' && s.exerciseName !== currentName) break
      j++
    }
    if (j >= steps.length) {
      complete()
    } else {
      setIndex(j)
    }
  }, [steps, complete])

  const restartStep = useCallback(() => {
    const step = steps[indexRef.current]
    if (!step) return
    const ms = step.durationSeconds * 1000
    remainingRef.current = ms
    setRemainingMs(ms)
    deadlineRef.current = performance.now() + ms
    setStatus('running')
  }, [steps])

  return {
    index,
    step: steps[index],
    remainingMs,
    status,
    totalSteps: steps.length,
    pause,
    resume,
    togglePause,
    skip,
    skipExercise,
    restartStep,
  }
}
