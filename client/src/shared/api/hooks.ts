import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from './client'
import type {
  BodyMetricDto,
  CreateSessionRequest,
  CreateWorkoutDayRequest,
  DietDayDto,
  EquipmentItemDto,
  LogBodyMetricRequest,
  MuscleDto,
  NutritionDto,
  PhaseTodayDto,
  SessionDto,
  SettingsDto,
  StatsSummaryDto,
  UpdateDietDayRequest,
  UpdateNutritionProfileRequest,
  UpdateSessionRequest,
  UpdateWorkoutDayRequest,
  UpsertEquipmentRequest,
  UpsertExerciseRequest,
  UpsertMealRequest,
  VideoLibraryItemDto,
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
  videos: ['videos'] as const,
  equipment: ['equipment'] as const,
  muscles: ['muscles'] as const,
  nutrition: ['nutrition'] as const,
  bodyMetrics: (from?: string, to?: string) => ['body-metrics', { from, to }] as const,
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

// ---- Workout configuration (full CRUD) ----

export function useVideoLibrary() {
  return useQuery({
    queryKey: queryKeys.videos,
    queryFn: () => api.get<VideoLibraryItemDto[]>('/api/videos'),
    staleTime: 5 * 60_000,
  })
}

function invalidateProgram(qc: ReturnType<typeof useQueryClient>) {
  qc.invalidateQueries({ queryKey: ['workout-days'] })
  qc.invalidateQueries({ queryKey: queryKeys.stats })
}

export function useCreateDay() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: CreateWorkoutDayRequest) => api.post<WorkoutDayDto>('/api/workout-days', body),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useUpdateDay() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateWorkoutDayRequest }) =>
      api.put<WorkoutDayDto>(`/api/workout-days/${id}`, body),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useDeleteDay() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => api.del<void>(`/api/workout-days/${id}`),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useReorderDays() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (orderedIds: number[]) =>
      api.post<void>('/api/workout-days/reorder', { orderedIds }),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useCreateExercise() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, body }: { dayId: number; body: UpsertExerciseRequest }) =>
      api.post<WorkoutDayDto>(`/api/workout-days/${dayId}/exercises`, body),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useUpdateExercise() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      dayId,
      exerciseId,
      body,
    }: {
      dayId: number
      exerciseId: number
      body: UpsertExerciseRequest
    }) => api.put<WorkoutDayDto>(`/api/workout-days/${dayId}/exercises/${exerciseId}`, body),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useDeleteExercise() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, exerciseId }: { dayId: number; exerciseId: number }) =>
      api.del<void>(`/api/workout-days/${dayId}/exercises/${exerciseId}`),
    onSuccess: () => invalidateProgram(qc),
  })
}

export function useReorderExercises() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, orderedIds }: { dayId: number; orderedIds: number[] }) =>
      api.post<void>(`/api/workout-days/${dayId}/exercises/reorder`, { orderedIds }),
    onSuccess: () => invalidateProgram(qc),
  })
}

/** Appends the latest seeded program template to the user's program (non-destructive). */
export function useImportTemplate() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: () => api.post<WorkoutDayDto[]>('/api/workout-days/import-template', {}),
    onSuccess: () => invalidateProgram(qc),
  })
}

// ---- Equipment inventory ----

export function useEquipment() {
  return useQuery({
    queryKey: queryKeys.equipment,
    queryFn: () => api.get<EquipmentItemDto[]>('/api/equipment'),
  })
}

function invalidateEquipment(qc: ReturnType<typeof useQueryClient>) {
  qc.invalidateQueries({ queryKey: queryKeys.equipment })
}

export function useCreateEquipment() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: UpsertEquipmentRequest) => api.post<EquipmentItemDto>('/api/equipment', body),
    onSuccess: () => invalidateEquipment(qc),
  })
}

export function useUpdateEquipment() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertEquipmentRequest }) =>
      api.put<EquipmentItemDto>(`/api/equipment/${id}`, body),
    onSuccess: () => invalidateEquipment(qc),
  })
}

export function useDeleteEquipment() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => api.del<void>(`/api/equipment/${id}`),
    onSuccess: () => invalidateEquipment(qc),
  })
}

// ---- Muscle catalog ----

export function useMuscles() {
  return useQuery({
    queryKey: queryKeys.muscles,
    queryFn: () => api.get<MuscleDto[]>('/api/muscles'),
    staleTime: 60 * 60_000,
  })
}

// ---- Nutrition ----

export function useNutrition() {
  return useQuery({
    queryKey: queryKeys.nutrition,
    queryFn: () => api.get<NutritionDto>('/api/nutrition'),
  })
}

function invalidateNutrition(qc: ReturnType<typeof useQueryClient>) {
  qc.invalidateQueries({ queryKey: queryKeys.nutrition })
}

export function useUpdateNutritionProfile() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: UpdateNutritionProfileRequest) => api.put('/api/nutrition/profile', body),
    onSuccess: () => invalidateNutrition(qc),
  })
}

export function useUpdateDietDay() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateDietDayRequest }) =>
      api.put<DietDayDto>(`/api/nutrition/days/${id}`, body),
    onSuccess: () => invalidateNutrition(qc),
  })
}

export function useCreateMeal() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, body }: { dayId: number; body: UpsertMealRequest }) =>
      api.post<DietDayDto>(`/api/nutrition/days/${dayId}/meals`, body),
    onSuccess: () => invalidateNutrition(qc),
  })
}

export function useUpdateMeal() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, mealId, body }: { dayId: number; mealId: number; body: UpsertMealRequest }) =>
      api.put<DietDayDto>(`/api/nutrition/days/${dayId}/meals/${mealId}`, body),
    onSuccess: () => invalidateNutrition(qc),
  })
}

export function useDeleteMeal() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ dayId, mealId }: { dayId: number; mealId: number }) =>
      api.del<void>(`/api/nutrition/days/${dayId}/meals/${mealId}`),
    onSuccess: () => invalidateNutrition(qc),
  })
}

// ---- Bodyweight ----

export function useBodyMetrics(from?: string, to?: string) {
  const params = new URLSearchParams()
  if (from) params.set('from', from)
  if (to) params.set('to', to)
  const qs = params.toString()
  return useQuery({
    queryKey: queryKeys.bodyMetrics(from, to),
    queryFn: () => api.get<BodyMetricDto[]>(`/api/body-metrics${qs ? `?${qs}` : ''}`),
  })
}

export function useLogBodyMetric() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: LogBodyMetricRequest) => api.post<BodyMetricDto>('/api/body-metrics', body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['body-metrics'] })
      qc.invalidateQueries({ queryKey: queryKeys.nutrition })
    },
  })
}

export function useDeleteBodyMetric() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => api.del<void>(`/api/body-metrics/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['body-metrics'] }),
  })
}
