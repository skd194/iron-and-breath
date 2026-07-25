import type { SessionDto, WorkoutDayDto } from '../../shared/api/types'

/** Completed session dates as a Set of yyyy-MM-dd for the heatmap. */
export function completedDateSet(sessions: SessionDto[]): Set<string> {
  return new Set(sessions.filter((s) => s.completedAt != null).map((s) => s.date))
}

/**
 * The next day in the 4-day rotation after the most recently completed session.
 * Falls back to the first day when there's no history.
 */
export function suggestedDayId(days: WorkoutDayDto[], sessions: SessionDto[]): number | null {
  if (days.length === 0) return null
  const ordered = [...days].sort((a, b) => a.sortOrder - b.sortOrder)

  const lastCompleted = sessions
    .filter((s) => s.completedAt != null)
    .sort((a, b) => b.date.localeCompare(a.date) || b.startedAt.localeCompare(a.startedAt))[0]

  if (!lastCompleted) return ordered[0].id

  const idx = ordered.findIndex((d) => d.id === lastCompleted.workoutDayId)
  if (idx === -1) return ordered[0].id

  return ordered[(idx + 1) % ordered.length].id
}
