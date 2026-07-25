import { useMemo } from 'react'
import { Link, useParams } from 'react-router-dom'
import { usePhaseToday, useWarmUp, useWorkoutDay } from '../../shared/api/hooks'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import { RunningSession } from './RunningSession'
import { buildSessionPlan } from './session-plan'
import './player.css'

export function SessionPlayerPage() {
  const { dayId } = useParams()
  const id = Number(dayId)

  const dayQ = useWorkoutDay(Number.isFinite(id) ? id : undefined)
  const phaseQ = usePhaseToday()
  const warmupQ = useWarmUp()

  const steps = useMemo(() => {
    if (!dayQ.data || !phaseQ.data || !warmupQ.data) return []
    return buildSessionPlan(dayQ.data, phaseQ.data, warmupQ.data)
  }, [dayQ.data, phaseQ.data, warmupQ.data])

  if (dayQ.isLoading || phaseQ.isLoading || warmupQ.isLoading) {
    return (
      <div className="player-shell">
        <LoadingState label="Preparing your session…" />
      </div>
    )
  }

  if (dayQ.isError || !dayQ.data || !phaseQ.data || !warmupQ.data) {
    return (
      <div className="player-shell">
        <ErrorState message="Couldn't load this workout." />
        <div style={{ textAlign: 'center' }}>
          <Link className="btn btn-ghost" to="/">
            Back to dashboard
          </Link>
        </div>
      </div>
    )
  }

  if (steps.length === 0) {
    return (
      <div className="player-shell">
        <ErrorState message="This day has no exercises configured." />
      </div>
    )
  }

  return (
    <div className="player-shell">
      <RunningSession
        steps={steps}
        dayId={dayQ.data.id}
        dayName={dayQ.data.name}
        phaseNumber={phaseQ.data.phaseNumber}
        weekNumber={phaseQ.data.weekNumber}
      />
    </div>
  )
}
