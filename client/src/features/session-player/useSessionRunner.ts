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
  restartStep: () => void
}

interface Options {
  onComplete?: () => void
  /** Fired when a new step becomes active (including the first). Good for audio cues. */
  onStepStart?: (step: SessionStep, index: number) => void
}

/**
 * Drives the scripted session: a monotonic countdown per step that auto-advances
 * at zero. The timer is the source of truth for pacing. Uses a wall-clock
 * deadline so it stays accurate regardless of tick jitter, and refs to avoid
 * stale closures inside the interval.
 */
export function useSessionRunner(steps: SessionStep[], options: Options = {}): SessionRunner {
  const [index, setIndex] = useState(0)
  const [status, setStatus] = useState<RunnerStatus>('running')
  const [remainingMs, setRemainingMs] = useState(() => (steps[0]?.durationSeconds ?? 0) * 1000)

  const deadlineRef = useRef<number>(0)
  const remainingRef = useRef<number>(remainingMs)
  const indexRef = useRef(0)
  const statusRef = useRef<RunnerStatus>('running')

  const onCompleteRef = useRef(options.onComplete)
  const onStepStartRef = useRef(options.onStepStart)
  onCompleteRef.current = options.onComplete
  onStepStartRef.current = options.onStepStart

  indexRef.current = index
  statusRef.current = status
  remainingRef.current = remainingMs

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

  const advance = useCallback(() => {
    const i = indexRef.current
    if (i < steps.length - 1) {
      setIndex(i + 1)
    } else {
      setStatus('complete')
      onCompleteRef.current?.()
    }
  }, [steps.length])

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
    restartStep,
  }
}
