import { useCallback, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateSession } from '../../shared/api/hooks'
import type { CreateSessionRequest, CreateSetLogRequest } from '../../shared/api/types'
import { ProgressRing } from '../../shared/ui/ProgressRing'
import { todayIso } from '../../shared/util/date'
import { cueComplete, cueTransition, unlockAudio } from './audio'
import { BreathingIndicator } from './components/BreathingIndicator'
import { ExerciseAnimation } from './components/ExerciseAnimation'
import { SessionAgenda } from './components/SessionAgenda'
import { TargetMuscles } from './components/TargetMuscles'
import type { SessionStep } from './session-plan'
import { useSessionRunner } from './useSessionRunner'
import { useVoiceCommands, type VoiceIntent } from './voice/useVoiceCommands'

interface RunningSessionProps {
  steps: SessionStep[]
  dayId: number
  dayName: string
  phaseNumber: number
  weekNumber: number
}

const KIND_ACCENT: Record<SessionStep['kind'], string> = {
  'warmup-round': 'var(--ember-500)',
  'warmup-transition': 'var(--gold-500)',
  work: 'var(--breath-500)',
  'rest-set': 'var(--breath-600)',
  'rest-exercise': 'var(--breath-600)',
}

function formatTime(ms: number): string {
  const total = Math.ceil(ms / 1000)
  const m = Math.floor(total / 60)
  const s = total % 60
  return `${m}:${String(s).padStart(2, '0')}`
}

export function RunningSession({ steps, dayId, dayName, phaseNumber, weekNumber }: RunningSessionProps) {
  const navigate = useNavigate()
  const createSession = useCreateSession()
  const startedAtRef = useRef(new Date())
  const postedRef = useRef(false)
  // Set logs accumulate as each work set actually starts (skipped-past sets are
  // never recorded), keyed by step id so re-entering a set doesn't duplicate.
  const setLogsRef = useRef(new Map<string, CreateSetLogRequest>())
  const [finished, setFinished] = useState(false)
  const [confirmExit, setConfirmExit] = useState(false)

  // Resume the same session if the user navigates away and comes back.
  const persistKey = `ib_run_${dayId}`

  const buildPayload = useCallback(
    (): CreateSessionRequest => ({
      workoutDayId: dayId,
      date: todayIso(),
      startedAt: startedAtRef.current.toISOString(),
      completedAt: new Date().toISOString(),
      source: 'Guided',
      setLogs: Array.from(setLogsRef.current.values()),
    }),
    [dayId],
  )

  const runner = useSessionRunner(steps, {
    persistKey,
    onStepStart: (step) => {
      unlockAudio()
      cueTransition()
      // Record the set as performed (reps default to the plan; weights are
      // entered via manual log / history for now).
      if (step.kind === 'work' && step.exerciseId != null && step.setNumber != null) {
        setLogsRef.current.set(step.id, {
          exerciseId: step.exerciseId,
          exerciseName: step.exerciseName ?? null,
          setNumber: step.setNumber,
          repsCompleted: step.plannedReps ?? null,
        })
      }
    },
    onComplete: () => {
      cueComplete()
      setFinished(true)
      if (!postedRef.current) {
        postedRef.current = true
        createSession.mutate(buildPayload())
      }
    },
  })

  // Voice seam: spoken phrases drive the same controls. Opt-in via the mic
  // button; recognition only runs where the browser supports it.
  const handleIntent = useCallback(
    (intent: VoiceIntent) => {
      switch (intent) {
        case 'start':
        case 'resume':
          runner.resume()
          break
        case 'pause':
          runner.pause()
          break
        case 'skip':
          runner.skip()
          break
        case 'next-exercise':
          runner.skipExercise()
          break
        case 'restart':
          runner.restartStep()
          break
        case 'stop':
          setConfirmExit(true)
          break
        default:
          // Informational intents (reps/rest/muscle questions) get spoken
          // answers in a later phase.
          break
      }
    },
    [runner],
  )
  const voice = useVoiceCommands(handleIntent, { enabled: true })

  const exitSession = useCallback(() => {
    try {
      sessionStorage.removeItem(persistKey)
    } catch {
      /* non-fatal */
    }
    navigate('/')
  }, [navigate, persistKey])

  const summary = useMemo(() => {
    const workSteps = steps.filter((s) => s.kind === 'work')
    const exercises = new Set(workSteps.map((s) => s.exerciseName))
    return { totalSets: workSteps.length, exerciseCount: exercises.size }
  }, [steps])

  if (finished) {
    const mins = Math.round((Date.now() - startedAtRef.current.getTime()) / 60000)
    return (
      <div className="player finished">
        <div className="finish-card card card-pad">
          <div className="finish-check">✓</div>
          <h1>Session complete</h1>
          <p className="muted">{dayName}</p>
          <div className="finish-stats">
            <div>
              <strong>{summary.exerciseCount}</strong>
              <span className="muted">exercises</span>
            </div>
            <div>
              <strong>{summary.totalSets}</strong>
              <span className="muted">sets</span>
            </div>
            <div>
              <strong>~{mins}m</strong>
              <span className="muted">elapsed</span>
            </div>
            <div>
              <strong>P{phaseNumber}·W{weekNumber}</strong>
              <span className="muted">phase</span>
            </div>
          </div>
          <div className="finish-save muted">
            {createSession.isPending && 'Saving…'}
            {createSession.isSuccess && 'Logged ✓'}
            {createSession.isError && (
              <span style={{ color: 'var(--danger-500)' }}>
                Couldn't save — {' '}
                <button className="linklike" onClick={() => createSession.mutate(buildPayload())}>retry</button>
              </span>
            )}
          </div>
          <button className="btn btn-primary btn-lg" onClick={() => navigate('/')}>
            Back to dashboard
          </button>
        </div>
      </div>
    )
  }

  const step = runner.step
  if (!step) return null

  const accent = KIND_ACCENT[step.kind]
  const progress = step.durationSeconds > 0 ? runner.remainingMs / (step.durationSeconds * 1000) : 0
  const isWork = step.kind === 'work'
  const paused = runner.status === 'paused'

  return (
    <div className="player" style={{ ['--accent' as string]: accent }}>
      {/* Header */}
      <div className="player-top">
        <button className="btn btn-ghost" onClick={() => setConfirmExit(true)}>
          ✕ Exit
        </button>
        <div className="player-progress-info">
          {voice.supported && (
            <button
              className={`btn btn-ghost btn-sm mic-btn${voice.listening ? ' listening' : ''}`}
              onClick={voice.toggle}
              title={voice.listening ? 'Stop voice commands' : 'Voice commands (say "pause", "skip"…)'}
              aria-pressed={voice.listening}
            >
              {voice.listening ? '🎙️ Listening' : '🎙️ Voice'}
            </button>
          )}
          <span className="badge">
            Step {runner.index + 1} / {runner.totalSteps}
          </span>
          <span className="badge badge-accent">{dayName}</span>
        </div>
      </div>

      {/* Step progress bar */}
      <div className="steps-bar" aria-hidden>
        {steps.map((s, i) => (
          <span
            key={s.id}
            className={`steps-tick${i < runner.index ? ' past' : ''}${i === runner.index ? ' current' : ''}${s.isRest ? ' rest' : ''}`}
          />
        ))}
      </div>

      {/* Body: timer + info on the left, full plan list on the right (below on mobile) */}
      <div className="player-body">
        <div className="player-col-main">
          <div className={`player-main${step.isRest ? ' rest' : ''}`}>
            <div className="player-ring">
              <ProgressRing progress={progress} size={280} stroke={16} color={accent}>
                <div className="timer-value mono">{formatTime(runner.remainingMs)}</div>
                <div className="timer-kind">{labelForKind(step.kind)}</div>
              </ProgressRing>
            </div>

            {isWork ? (
              <WorkPanel step={step} />
            ) : step.isRest ? (
              <RestPanel step={step} />
            ) : (
              <div className="player-info">
                <h1 className="player-title">{step.title}</h1>
                {step.subtitle && <p className="player-subtitle">{step.subtitle}</p>}
              </div>
            )}
          </div>

          {/* Controls */}
          <div className="player-controls">
            <button className="btn btn-ghost" onClick={runner.restartStep} title="Restart this step">
              ↺ Restart
            </button>
            <button className="btn btn-primary btn-lg" onClick={runner.togglePause}>
              {paused ? '▶ Resume' : '⏸ Pause'}
            </button>
            <button className="btn btn-ghost" onClick={runner.skip} title="Skip to next step">
              Skip ⏭
            </button>
            {isWork && (
              <button
                className="btn btn-ghost"
                onClick={runner.skipExercise}
                title="Skip the rest of this exercise"
              >
                Skip exercise ⏭⏭
              </button>
            )}
          </div>
        </div>

        <aside className="player-agenda">
          <SessionAgenda steps={steps} currentIndex={runner.index} />
        </aside>
      </div>

      {paused && <div className="paused-veil">Paused</div>}

      {/* Exit confirmation */}
      {confirmExit && (
        <div className="modal-overlay" onClick={() => setConfirmExit(false)}>
          <div className="modal card card-pad" onClick={(e) => e.stopPropagation()}>
            <h2>Exit session?</h2>
            <p className="muted">This session won't be logged.</p>
            <div className="modal-actions">
              <button className="btn btn-ghost" onClick={() => setConfirmExit(false)}>
                Keep going
              </button>
              <button className="btn btn-danger" onClick={exitSession}>
                Exit without saving
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

/** The active-set view: reps, animation, breathing, target muscles, cue. */
function WorkPanel({ step }: { step: SessionStep }) {
  return (
    <div className="player-info">
      <h1 className="player-title">{step.title}</h1>
      {step.subtitle && <p className="player-subtitle">{step.subtitle}</p>}
      {step.repsDisplay && <div className="player-reps">{step.repsDisplay}</div>}
      <ExerciseAnimation animationRef={step.animationRef} video={step.video} name={step.title} />
      <BreathingIndicator breathing={step.breathing} />
      <TargetMuscles primary={step.primaryMuscles} secondary={step.secondaryMuscles} />
      {step.cue && <p className="player-cue">💡 {step.cue}</p>}
    </div>
  )
}

/** The rest view: what's coming up + useful context, without being distracting. */
function RestPanel({ step }: { step: SessionStep }) {
  return (
    <div className="player-info rest-panel">
      <h1 className="player-title">Rest</h1>
      {step.exerciseName && (
        <>
          <p className="rest-next-label muted">Up next</p>
          <p className="rest-next-name">{step.exerciseName}</p>
        </>
      )}
      {step.subtitle && <p className="player-subtitle">{step.subtitle}</p>}
      <TargetMuscles primary={step.primaryMuscles} secondary={step.secondaryMuscles} compact />
      {step.cue && <p className="player-cue">💡 {step.cue}</p>}
      {step.commonMistakes && (
        <p className="rest-tip">
          <span className="rest-tip-label">Avoid</span> {step.commonMistakes}
        </p>
      )}
      {step.benefits && (
        <p className="rest-tip">
          <span className="rest-tip-label">Why</span> {step.benefits}
        </p>
      )}
    </div>
  )
}

function labelForKind(kind: SessionStep['kind']): string {
  switch (kind) {
    case 'warmup-round':
      return 'warm-up'
    case 'warmup-transition':
      return 'transition'
    case 'work':
      return 'work'
    default:
      return 'rest'
  }
}
