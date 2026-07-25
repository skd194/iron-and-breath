import { Link } from 'react-router-dom'
import { usePhaseToday, useSessions, useStatsSummary, useWorkoutDays } from '../../shared/api/hooks'
import { ProgressRing } from '../../shared/ui/ProgressRing'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import { formatLong, todayIso } from '../../shared/util/date'
import { CalendarHeatmap } from './components/CalendarHeatmap'
import { DayPicker } from './components/DayPicker'
import { completedDateSet, suggestedDayId } from './selectors'
import './dashboard.css'

export function DashboardPage() {
  const daysQ = useWorkoutDays()
  const phaseQ = usePhaseToday()
  const statsQ = useStatsSummary()
  const sessionsQ = useSessions()

  if (daysQ.isLoading || phaseQ.isLoading || statsQ.isLoading || sessionsQ.isLoading) {
    return <LoadingState label="Loading your program…" />
  }
  if (daysQ.isError || !daysQ.data) {
    return <ErrorState message="Couldn't load workout days." onRetry={() => daysQ.refetch()} />
  }

  const days = daysQ.data
  const phase = phaseQ.data
  const stats = statsQ.data
  const sessions = sessionsQ.data ?? []

  const suggestedId = suggestedDayId(days, sessions)
  const suggestedDay = days.find((d) => d.id === suggestedId) ?? null
  const completed = completedDateSet(sessions)

  const monthlyDone = stats?.sessionsThisMonth ?? 0
  const monthlyTarget = stats?.monthlyTarget ?? 16
  const monthProgress = monthlyTarget > 0 ? monthlyDone / monthlyTarget : 0
  const weeklyTargetHint = Math.round((stats?.monthlyTarget ?? 16) / 4)

  return (
    <div className="dashboard grid">
      {/* Hero: today's suggested workout */}
      <section className="card card-pad hero">
        <div className="hero-left">
          <div className="section-title">Today · {formatLong(todayIso())}</div>
          {suggestedDay ? (
            <>
              <h1 className="hero-title">{suggestedDay.name}</h1>
              <p className="hero-focus">{suggestedDay.focus}</p>
              {phase && (
                <div className="hero-phase">
                  <span className="badge badge-accent">
                    Phase {phase.phaseNumber} · Week {phase.weekNumber}
                  </span>
                  <span className="muted">{phase.repsHintText}</span>
                </div>
              )}
              <div className="hero-actions">
                <Link className="btn btn-primary btn-lg" to={`/session/${suggestedDay.id}`}>
                  ▶ Start Workout
                </Link>
                <span className="muted">{suggestedDay.exercises.length} exercises</span>
              </div>
            </>
          ) : (
            <h1 className="hero-title">No workout days configured</h1>
          )}
        </div>
        <div className="hero-right">
          <ProgressRing progress={monthProgress} size={168} stroke={14}>
            <div className="ring-value">
              {monthlyDone}
              <span className="ring-target">/{monthlyTarget}</span>
            </div>
            <div className="ring-label">this month</div>
          </ProgressRing>
        </div>
      </section>

      {/* Stat cards */}
      <section className="stats-row">
        <StatCard label="Total sessions" value={stats?.totalSessions ?? 0} />
        <StatCard label="Weekly streak" value={`${stats?.currentWeeklyStreak ?? 0}w`} accent />
        <StatCard
          label="This week"
          value={`${stats?.sessionsThisWeek ?? 0}/${weeklyTargetHint}`}
        />
        <StatCard label="Avg / week" value={(stats?.averageSessionsPerWeek ?? 0).toFixed(1)} />
      </section>

      {/* Heatmap */}
      <section className="card card-pad">
        <div className="section-title">Last 35 days</div>
        <CalendarHeatmap completedDates={completed} />
      </section>

      {/* Day picker */}
      <section>
        <div className="section-title">The 4-day split</div>
        <DayPicker
          days={days}
          suggestedDayId={suggestedId}
          perDayCounts={stats?.perDayCompletedCounts ?? {}}
        />
      </section>
    </div>
  )
}

function StatCard({
  label,
  value,
  accent,
}: {
  label: string
  value: string | number
  accent?: boolean
}) {
  return (
    <div className="card card-pad statcard">
      <div className={`statcard-value${accent ? ' accent' : ''}`}>{value}</div>
      <div className="statcard-label">{label}</div>
    </div>
  )
}
