import { useEffect, useMemo, useRef } from 'react'
import type { SessionStep } from '../session-plan'
import { groupStepsForAgenda } from '../session-plan'

interface SessionAgendaProps {
  steps: SessionStep[]
  currentIndex: number
}

function fmt(seconds: number): string {
  const m = Math.floor(seconds / 60)
  const s = seconds % 60
  return m > 0 ? `${m}:${String(s).padStart(2, '0')}` : `${s}s`
}

/**
 * The full session grouped by exercise (plus a warm-up card) beside the timer:
 * each card shows reps, set count and work/rest timing, the active card is
 * highlighted with its live set, and done cards are checked off. Read-only —
 * the session must be worked through in order, so cards aren't tappable.
 * Auto-scrolls the active card into view.
 */
export function SessionAgenda({ steps, currentIndex }: SessionAgendaProps) {
  const groups = useMemo(() => groupStepsForAgenda(steps), [steps])
  const currentRef = useRef<HTMLLIElement>(null)

  useEffect(() => {
    currentRef.current?.scrollIntoView({ block: 'nearest', behavior: 'smooth' })
  }, [currentIndex])

  const remainingSeconds = steps
    .slice(currentIndex)
    .reduce((total, s) => total + s.durationSeconds, 0)
  const active = steps[currentIndex]
  const hasWarmup = groups[0]?.kind === 'warmup'

  return (
    <div className="agenda">
      <div className="agenda-head">
        <span className="agenda-title">Workout plan</span>
        <span className="agenda-sub muted">~{fmt(remainingSeconds)} left</span>
      </div>
      <ol className="agenda-list">
        {groups.map((g, gi) => {
          const state =
            currentIndex > g.endIndex ? 'done' : currentIndex >= g.startIndex ? 'current' : 'upcoming'
          const isExercise = g.kind === 'exercise'

          // Live sub-status for the active card.
          let liveNote: string | null = null
          if (state === 'current' && active) {
            if (active.kind === 'work' && active.setNumber && active.totalSets) {
              liveNote = `Set ${active.setNumber} of ${active.totalSets}`
            } else if (active.isRest) {
              liveNote = 'Resting'
            } else if (active.kind === 'warmup-round' && active.roundNumber && active.totalRounds) {
              liveNote = `Round ${active.roundNumber} of ${active.totalRounds}`
            }
          }

          return (
            <li
              key={g.key}
              ref={state === 'current' ? currentRef : undefined}
              className={`agenda-card ${state} ${g.kind}`}
              aria-current={state === 'current' ? 'step' : undefined}
            >
              <div className="agenda-card-head">
                  <span className="agenda-marker" aria-hidden>
                    {state === 'done' ? '✓' : isExercise ? (hasWarmup ? gi : gi + 1) : '☀'}
                  </span>
                  <span className="agenda-name">{g.title}</span>
                  <span className="agenda-total mono">{fmt(g.totalSeconds)}</span>
                </div>

                <span className="agenda-chips">
                  {isExercise ? (
                    <>
                      {g.sets != null && <span className="agenda-chip">{g.sets} sets</span>}
                      {g.reps && <span className="agenda-chip reps">{g.reps} reps</span>}
                      {g.workSeconds != null && (
                        <span className="agenda-chip">{fmt(g.workSeconds)} work</span>
                      )}
                      {g.restSeconds != null && (
                        <span className="agenda-chip">{fmt(g.restSeconds)} rest</span>
                      )}
                    </>
                  ) : (
                    <>
                      {g.rounds != null && <span className="agenda-chip">{g.rounds} rounds</span>}
                      {g.roundSeconds != null && (
                        <span className="agenda-chip">{fmt(g.roundSeconds)} / round</span>
                      )}
                    </>
                  )}
                </span>

                {liveNote && <span className="agenda-live">{liveNote}</span>}
                {isExercise && g.cue && state !== 'done' && (
                  <span className="agenda-cue">💡 {g.cue}</span>
                )}
            </li>
          )
        })}
      </ol>
    </div>
  )
}
