import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  useBodyMetrics,
  useLogBodyMetric,
  usePhaseToday,
  useSessions,
  useStatsSummary,
  useWorkoutDays,
} from '../../shared/api/hooks'
import type { SessionDto, WorkoutDayDto } from '../../shared/api/types'
import { BarChart, type Bar } from '../../shared/ui/charts/BarChart'
import { LineChart, type LinePoint } from '../../shared/ui/charts/LineChart'
import { ProgressRing } from '../../shared/ui/ProgressRing'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import { addDays, formatLong, toIsoDate, todayIso } from '../../shared/util/date'
import { CalendarHeatmap } from './components/CalendarHeatmap'
import { DayPicker } from './components/DayPicker'
import { adherence, completedDateSet, hasSchedule, missedDateSet, suggestedDayId } from './selectors'
import './dashboard.css'

const RANGES = [35, 90, 180, 365] as const
const WEEK_ORDER = [1, 2, 3, 4, 5, 6, 0] // Mon..Sun
const WEEKDAY_SHORT = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

export function DashboardPage() {
  const daysQ = useWorkoutDays()
  const phaseQ = usePhaseToday()
  const statsQ = useStatsSummary()
  const sessionsQ = useSessions()

  const [range, setRange] = useState<number>(90)

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

  const scheduled = hasSchedule(days)
  const suggestedId = suggestedDayId(days, sessions)
  const suggestedDay = days.find((d) => d.id === suggestedId) ?? null
  const completed = completedDateSet(sessions)
  const missed = missedDateSet(days, sessions, phase?.programStartDate, range)
  const adh = adherence(days, sessions, phase?.programStartDate, range)

  const monthlyDone = stats?.sessionsThisMonth ?? 0
  const monthlyTarget = stats?.monthlyTarget ?? 20
  const monthProgress = monthlyTarget > 0 ? monthlyDone / monthlyTarget : 0
  const weeklyTargetHint = Math.round((stats?.monthlyTarget ?? 20) / 4)

  const isRestToday = scheduled && !suggestedDay

  return (
    <div className="dashboard grid">
      {/* Hero: today's scheduled workout (or a rest day) */}
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
          ) : isRestToday ? (
            <>
              <h1 className="hero-title">Rest day</h1>
              <p className="hero-focus">Recover, stretch, hydrate. Nothing scheduled today.</p>
              <div className="hero-actions">
                <Link className="btn btn-ghost btn-lg" to="/nutrition">
                  View today's nutrition
                </Link>
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
        <StatCard label="This week" value={`${stats?.sessionsThisWeek ?? 0}/${weeklyTargetHint}`} />
        <StatCard
          label="Adherence"
          value={adh.rate != null ? `${Math.round(adh.rate * 100)}%` : '—'}
          accent
        />
      </section>

      {/* Weekly schedule strip */}
      {scheduled && (
        <section className="card card-pad">
          <div className="section-title">Your week</div>
          <WeekStrip days={days} />
        </section>
      )}

      {/* Heatmap with range selector */}
      <section className="card card-pad">
        <div className="section-head-row">
          <div className="section-title" style={{ margin: 0 }}>
            Last {range} days
          </div>
          <div className="range-picker">
            {RANGES.map((r) => (
              <button
                key={r}
                className={`chip${r === range ? ' chip-active' : ''}`}
                onClick={() => setRange(r)}
              >
                {r}
              </button>
            ))}
          </div>
        </div>
        <CalendarHeatmap completedDates={completed} missedDates={missed} days={range} />
        <div className="heatmap-legend muted">
          <span><i className="dot done" /> completed</span>
          <span><i className="dot missed" /> missed</span>
          {adh.rate != null && (
            <span className="legend-adh">
              {adh.completed}/{adh.scheduled} scheduled done
            </span>
          )}
        </div>
      </section>

      {/* Progress: bodyweight trend + weekly sessions */}
      <section className="progress-grid">
        <BodyweightCard />
        <div className="card card-pad">
          <div className="section-title">Weekly volume (kg) · last 12 weeks</div>
          <BarChart bars={weeklyVolume(sessions)} />
        </div>
      </section>

      {/* Day picker */}
      <section>
        <div className="section-title">Your program · {days.length} days</div>
        <DayPicker
          days={days}
          suggestedDayId={suggestedId}
          perDayCounts={stats?.perDayCompletedCounts ?? {}}
        />
      </section>
    </div>
  )
}

function WeekStrip({ days }: { days: WorkoutDayDto[] }) {
  const todayDow = new Date().getDay()
  return (
    <div className="week-strip">
      {WEEK_ORDER.map((dow) => {
        const day = days.find((d) => d.scheduledDayOfWeek === dow) ?? null
        return (
          <div key={dow} className={`week-cell${dow === todayDow ? ' today' : ''}${day ? '' : ' rest'}`}>
            <span className="week-dow">{WEEKDAY_SHORT[dow]}</span>
            {day ? (
              <Link to={`/session/${day.id}`} className="week-name" title={day.name}>
                {day.name.split('·')[0].trim()}
              </Link>
            ) : (
              <span className="week-name muted">Rest</span>
            )}
          </div>
        )
      })}
    </div>
  )
}

function BodyweightCard() {
  const metricsQ = useBodyMetrics()
  const log = useLogBodyMetric()
  const [weight, setWeight] = useState('')

  const points: LinePoint[] = useMemo(
    () =>
      (metricsQ.data ?? []).map((m) => ({
        label: m.date.slice(5), // MM-DD
        value: m.weightKg,
      })),
    [metricsQ.data],
  )

  const latest = metricsQ.data && metricsQ.data.length > 0 ? metricsQ.data[metricsQ.data.length - 1] : null

  const submit = () => {
    const w = parseFloat(weight)
    if (!Number.isFinite(w) || w <= 0) return
    log.mutate({ date: todayIso(), weightKg: w }, { onSuccess: () => setWeight('') })
  }

  return (
    <div className="card card-pad">
      <div className="section-head-row">
        <div className="section-title" style={{ margin: 0 }}>
          Bodyweight
        </div>
        {latest && <span className="mono muted">{latest.weightKg} kg</span>}
      </div>
      <LineChart points={points} unit="kg" minSpan={2} />
      <div className="weight-log">
        <input
          type="number"
          inputMode="decimal"
          step="0.1"
          placeholder="Today's weight (kg)"
          value={weight}
          onChange={(e) => setWeight(e.target.value)}
          className="input"
        />
        <button className="btn btn-primary btn-sm" onClick={submit} disabled={log.isPending || !weight}>
          {log.isPending ? 'Saving…' : 'Log'}
        </button>
      </div>
    </div>
  )
}

/**
 * Weekly training volume (volume load = Σ weight × reps) over the last `weeks`
 * Monday-anchored weeks — the clearest measure of progressive overload. Only
 * counts sets with a logged weight, so it climbs as you log weights.
 */
function weeklyVolume(sessions: SessionDto[], weeks = 12): Bar[] {
  const today = new Date()
  const monday = (d: Date) => addDays(d, -(((d.getDay() + 6) % 7)))
  const thisMonday = monday(today)

  const buckets: Bar[] = []
  for (let w = weeks - 1; w >= 0; w--) {
    const start = addDays(thisMonday, -7 * w)
    const startIso = toIsoDate(start)
    const endIso = toIsoDate(addDays(start, 6))
    const volume = sessions
      .filter((s) => s.completedAt != null && s.date >= startIso && s.date <= endIso)
      .reduce(
        (sum, s) =>
          sum +
          s.setLogs.reduce((v, log) => v + (log.weightKg ?? 0) * (log.repsCompleted ?? 0), 0),
        0,
      )
    buckets.push({
      label: start.getDate() === 1 || w === weeks - 1 ? startIso.slice(5) : `${start.getDate()}`,
      value: Math.round(volume),
    })
  }
  return buckets
}

function StatCard({ label, value, accent }: { label: string; value: string | number; accent?: boolean }) {
  return (
    <div className="card card-pad statcard">
      <div className={`statcard-value${accent ? ' accent' : ''}`}>{value}</div>
      <div className="statcard-label">{label}</div>
    </div>
  )
}
