import { useCallback, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateSession, useMuscles } from '../../shared/api/hooks'
import type { CreateSessionRequest, CreateSetLogRequest, MuscleDto } from '../../shared/api/types'
import { ProgressRing } from '../../shared/ui/ProgressRing'
import { todayIso } from '../../shared/util/date'
import { cueComplete, cueTransition, unlockAudio } from './audio'
import { BreathingIndicator } from './components/BreathingIndicator'
import { ExerciseAnimation } from './components/ExerciseAnimation'
import { RepTempoGuide } from './components/RepTempoGuide'
import { SessionAgenda } from './components/SessionAgenda'
import { SetLogForm } from './components/SetLogForm'
import { TargetMuscles } from './components/TargetMuscles'
import { toRepTempo } from './rep-tempo'
import type { SessionStep } from './session-plan'
import { useSessionRunner } from './useSessionRunner'
import { useVoiceCommands, type VoiceIntent } from './voice/useVoiceCommands'

/** Case-insensitive lookup from muscle name to its catalog entry. */
export type MuscleLookup = (name: string) => MuscleDto | undefined

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
  const musclesQ = useMuscles()
  const muscleLookup = useMemo<MuscleLookup>(() => {
    const map = new Map((musclesQ.data ?? []).map((m) => [m.name.toLowerCase(), m]))
    return (name: string) => map.get(name.toLowerCase())
  }, [musclesQ.data])
  const startedAtRef = useRef(new Date())
  const postedRef = useRef(false)
  // Set logs accumulate as each work set actually starts (skipped-past sets are
  // never recorded), keyed by step id so re-entering a set doesn't duplicate.
  const setLogsRef = useRef(new Map<string, CreateSetLogRequest>())
  // The most recent work step, so the following rest screen can log that set.
  const lastWorkRef = useRef<SessionStep | null>(null)
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
      // Record the set as performed (reps default to the plan; the user enters
      // actual weight/reps on the following rest screen).
      if (step.kind === 'work' && step.exerciseId != null && step.setNumber != null) {
        lastWorkRef.current = step
        if (!setLogsRef.current.has(step.id)) {
          setLogsRef.current.set(step.id, {
            exerciseId: step.exerciseId,
            exerciseName: step.exerciseName ?? null,
            setNumber: step.setNumber,
            repsCompleted: step.plannedReps ?? null,
          })
        }
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
  const isRestBetween = step.kind === 'rest-set' || step.kind === 'rest-exercise'
  const paused = runner.status === 'paused'
  const hasTempo = isWork && !!toRepTempo(step.tempoSpec) && (step.plannedReps ?? 0) > 0

  const ring = (
    <div className="player-ring">
      <ProgressRing progress={progress} size={240} stroke={14} color={accent}>
        <div className="timer-value mono">{formatTime(runner.remainingMs)}</div>
        <div className="timer-kind">{labelForKind(step.kind)}</div>
      </ProgressRing>
    </div>
  )

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
          <div className={`player-main ${isWork ? 'work' : isRestBetween ? 'rest' : 'warmup'}`}>
            {/* Heading spans the full width above the stage */}
            <div className="player-heading">
              {isWork ? (
                <>
                  <h1 className="player-title">{step.title}</h1>
                  {step.subtitle && <p className="player-subtitle">{step.subtitle}</p>}
                  {step.repsDisplay && <div className="player-reps">{step.repsDisplay}</div>}
                </>
              ) : isRestBetween ? (
                <>
                  <h1 className="player-title">Rest</h1>
                  {step.exerciseName && (
                    <p className="player-subtitle">
                      Up next — <strong>{step.exerciseName}</strong>
                    </p>
                  )}
                </>
              ) : (
                <>
                  <span className="warmup-label">Warm-up</span>
                  <h1 className="player-title">{step.title}</h1>
                  {step.subtitle && <p className="player-subtitle">{step.subtitle}</p>}
                </>
              )}
            </div>

            {isWork ? (
              <div className="player-stage">
                <div className="stage-focus">
                  {ring}
                  {hasTempo ? (
                    <RepTempoGuide
                      tempoSpec={step.tempoSpec}
                      reps={step.plannedReps}
                      breathing={step.breathing}
                      totalSeconds={step.durationSeconds}
                      remainingMs={runner.remainingMs}
                    />
                  ) : (
                    <BreathingIndicator breathing={step.breathing} />
                  )}
                </div>
                <div className="stage-detail">
                  <WorkDetail step={step} />
                </div>
              </div>
            ) : isRestBetween ? (
              <>
                <div className="player-stage">
                  <div className="stage-focus">
                    {ring}
                    {lastWorkRef.current && (
                      <SetLogForm workStep={lastWorkRef.current} setLogsRef={setLogsRef} />
                    )}
                  </div>
                  <div className="stage-detail">
                    <RestDetail step={step} muscleLookup={muscleLookup} />
                  </div>
                </div>
                <RestMuscles step={step} muscleLookup={muscleLookup} />
              </>
            ) : (
              <div className="warmup-view">{ring}</div>
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

/** Work-set detail column: the form clip, target muscles, and the cue. */
function WorkDetail({ step }: { step: SessionStep }) {
  return (
    <div className="player-detail">
      <ExerciseAnimation animationRef={step.animationRef} video={step.video} name={step.title} />
      <TargetMuscles primary={step.primaryMuscles} secondary={step.secondaryMuscles} />
      {step.cue && <p className="player-cue">💡 {step.cue}</p>}
    </div>
  )
}

/**
 * Rest detail column: rehearse the upcoming movement's form and read its technique
 * tips while recovering — the animation, cue, safety and "why".
 */
function RestDetail({ step, muscleLookup: _muscleLookup }: { step: SessionStep; muscleLookup: MuscleLookup }) {
  return (
    <div className="player-detail rest-panel">
      {step.exerciseName && (
        <ExerciseAnimation animationRef={step.animationRef} video={step.video} name={step.exerciseName} />
      )}
      <TargetMuscles primary={step.primaryMuscles} secondary={step.secondaryMuscles} compact />

      {step.cue && <p className="player-cue">💡 {step.cue}</p>}
      {step.safetyTips && (
        <p className="rest-tip">
          <span className="rest-tip-label">Safety</span> {step.safetyTips}
        </p>
      )}
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

/** Full-width grid of "what each muscle does", below the rest stage. */
function RestMuscles({ step, muscleLookup }: { step: SessionStep; muscleLookup: MuscleLookup }) {
  const muscleEntries = useMemo(() => {
    const names = [...(step.primaryMuscles ?? []), ...(step.secondaryMuscles ?? [])]
    const seen = new Set<string>()
    return names
      .filter((n) => (seen.has(n.toLowerCase()) ? false : (seen.add(n.toLowerCase()), true)))
      .map((n) => muscleLookup(n))
      .filter((m): m is MuscleDto => m != null)
  }, [step.primaryMuscles, step.secondaryMuscles, muscleLookup])

  if (muscleEntries.length === 0) return null

  return (
    <div className="rest-muscles-section">
      <p className="rest-muscles-head muted">Muscles worked — read while you recover</p>
      <div className="rest-muscles-grid">
        {muscleEntries.map((m) => (
          <div key={m.id} className="rest-muscle">
            <span className="rest-muscle-name">{m.name}</span>
            <span className="rest-muscle-fn">{m.function}</span>
            {m.injuryNotes && <span className="rest-muscle-injury">⚠ {m.injuryNotes}</span>}
          </div>
        ))}
      </div>
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
