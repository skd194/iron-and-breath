import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from './client'
import type {
  CreateSessionRequest,
  PhaseTodayDto,
  SessionDto,
  SettingsDto,
  StatsSummaryDto,
  UpdateSessionRequest,
  WarmUpDto,
  WorkoutDayDto,
} from './types'

export const queryKeys = {
  workoutDays: ['workout-days'] as const,
  workoutDay: (id: number) => ['workout-days', id] as const,
  phaseToday: ['program', 'phase-today'] as const,
  warmup: ['program', 'warmup'] as const,
  settings: ['settings'] as const,
  stats: ['stats', 'summary'] as const,
  sessions: (from?: string, to?: string) => ['sessions', { from, to }] as const,
}

export function useWarmUp() {
  return useQuery({
    queryKey: queryKeys.warmup,
    queryFn: () => api.get<WarmUpDto>('/api/program/warmup'),
    staleTime: Infinity,
  })
}

export function useWorkoutDays() {
  return useQuery({
    queryKey: queryKeys.workoutDays,
    queryFn: () => api.get<WorkoutDayDto[]>('/api/workout-days'),
  })
}

export function useWorkoutDay(id: number | undefined) {
  return useQuery({
    queryKey: queryKeys.workoutDay(id ?? 0),
    queryFn: () => api.get<WorkoutDayDto>(`/api/workout-days/${id}`),
    enabled: id != null,
  })
}

export function usePhaseToday() {
  return useQuery({
    queryKey: queryKeys.phaseToday,
    queryFn: () => api.get<PhaseTodayDto>('/api/program/phase-today'),
  })
}

export function useStatsSummary() {
  return useQuery({
    queryKey: queryKeys.stats,
    queryFn: () => api.get<StatsSummaryDto>('/api/stats/summary'),
  })
}

export function useSettings() {
  return useQuery({
    queryKey: queryKeys.settings,
    queryFn: () => api.get<SettingsDto>('/api/settings'),
  })
}

export function useUpdateSettings() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: SettingsDto) => api.put<SettingsDto>('/api/settings', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: queryKeys.settings })
      qc.invalidateQueries({ queryKey: queryKeys.phaseToday })
      qc.invalidateQueries({ queryKey: queryKeys.stats })
    },
  })
}

export function useSessions(from?: string, to?: string) {
  const params = new URLSearchParams()
  if (from) params.set('from', from)
  if (to) params.set('to', to)
  const qs = params.toString()
  return useQuery({
    queryKey: queryKeys.sessions(from, to),
    queryFn: () => api.get<SessionDto[]>(`/api/sessions${qs ? `?${qs}` : ''}`),
  })
}

function invalidateSessionData(qc: ReturnType<typeof useQueryClient>) {
  qc.invalidateQueries({ queryKey: ['sessions'] })
  qc.invalidateQueries({ queryKey: queryKeys.stats })
}

export function useCreateSession() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: CreateSessionRequest) => api.post<SessionDto>('/api/sessions', body),
    onSuccess: () => invalidateSessionData(qc),
  })
}

export function useUpdateSession() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateSessionRequest }) =>
      api.patch<SessionDto>(`/api/sessions/${id}`, body),
    onSuccess: () => invalidateSessionData(qc),
  })
}

export function useDeleteSession() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => api.del<void>(`/api/sessions/${id}`),
    onSuccess: () => invalidateSessionData(qc),
  })
}
