import type { SessionDto, WorkoutDayDto } from '../../shared/api/types'
import { addDays, fromIsoDate, toIsoDate } from '../../shared/util/date'

/** Completed session dates as a Set of yyyy-MM-dd for the heatmap. */
export function completedDateSet(sessions: SessionDto[]): Set<string> {
  return new Set(sessions.filter((s) => s.completedAt != null).map((s) => s.date))
}

/** The workout scheduled for a given weekday (0=Sun..6=Sat), or null (a rest day). */
export function dayForWeekday(days: WorkoutDayDto[], weekday: number): WorkoutDayDto | null {
  return days.find((d) => d.scheduledDayOfWeek === weekday) ?? null
}

/** The workout scheduled for today, or null when today is a rest day. */
export function todaysScheduledDay(days: WorkoutDayDto[]): WorkoutDayDto | null {
  return dayForWeekday(days, new Date().getDay())
}

/** Whether any day carries a weekday schedule (vs the legacy unscheduled program). */
export function hasSchedule(days: WorkoutDayDto[]): boolean {
  return days.some((d) => d.scheduledDayOfWeek != null)
}

/**
 * Today's suggested workout. Schedule-aware when days carry weekdays (returns the
 * day scheduled for today, or null on a rest day); otherwise falls back to the
 * next day in the rotation after the most recently completed session.
 */
export function suggestedDayId(days: WorkoutDayDto[], sessions: SessionDto[]): number | null {
  if (days.length === 0) return null

  if (hasSchedule(days)) {
    return todaysScheduledDay(days)?.id ?? null
  }

  const ordered = [...days].sort((a, b) => a.sortOrder - b.sortOrder)
  const lastCompleted = sessions
    .filter((s) => s.completedAt != null)
    .sort((a, b) => b.date.localeCompare(a.date) || b.startedAt.localeCompare(a.startedAt))[0]
  if (!lastCompleted) return ordered[0].id
  const idx = ordered.findIndex((d) => d.id === lastCompleted.workoutDayId)
  if (idx === -1) return ordered[0].id
  return ordered[(idx + 1) % ordered.length].id
}

/**
 * Dates (yyyy-MM-dd) the user was scheduled to train but logged no completed
 * session — "missed" days. Derived, never stored: back-logging a session clears it.
 * Only past dates (up to yesterday) within the program and the last `maxDays`.
 */
export function missedDateSet(
  days: WorkoutDayDto[],
  sessions: SessionDto[],
  programStartIso: string | undefined,
  maxDays = 365,
): Set<string> {
  const scheduledWeekdays = new Set(
    days.map((d) => d.scheduledDayOfWeek).filter((w): w is number => w != null),
  )
  const result = new Set<string>()
  if (scheduledWeekdays.size === 0) return result

  const completed = completedDateSet(sessions)
  const today = new Date()
  const earliest = addDays(today, -(maxDays - 1))
  let cursor = programStartIso ? fromIsoDate(programStartIso) : earliest
  if (cursor < earliest) cursor = earliest
  const yesterday = addDays(today, -1)

  for (let d = cursor; d <= yesterday; d = addDays(d, 1)) {
    if (!scheduledWeekdays.has(d.getDay())) continue
    const iso = toIsoDate(d)
    if (!completed.has(iso)) result.add(iso)
  }
  return result
}

export interface Adherence {
  scheduled: number
  completed: number
  /** 0..1, or null when nothing was scheduled in the range. */
  rate: number | null
}

/** Attendance over the last `rangeDays`: scheduled vs actually completed. */
export function adherence(
  days: WorkoutDayDto[],
  sessions: SessionDto[],
  programStartIso: string | undefined,
  rangeDays: number,
): Adherence {
  const scheduledWeekdays = new Set(
    days.map((d) => d.scheduledDayOfWeek).filter((w): w is number => w != null),
  )
  if (scheduledWeekdays.size === 0) return { scheduled: 0, completed: 0, rate: null }

  const completed = completedDateSet(sessions)
  const today = new Date()
  let cursor = addDays(today, -(rangeDays - 1))
  const start = programStartIso ? fromIsoDate(programStartIso) : cursor
  if (cursor < start) cursor = start
  const yesterday = addDays(today, -1)

  let scheduled = 0
  let done = 0
  for (let d = cursor; d <= yesterday; d = addDays(d, 1)) {
    if (!scheduledWeekdays.has(d.getDay())) continue
    scheduled++
    if (completed.has(toIsoDate(d))) done++
  }
  return { scheduled, completed: done, rate: scheduled > 0 ? done / scheduled : null }
}
