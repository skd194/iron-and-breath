// Mirrors the API DTOs (IronAndBreath.Api/Dtos).

export interface VideoDto {
  embedUrl: string
  thumbnailUrl: string | null
  title: string
  durationSeconds: number | null
  attribution: string | null
}

export interface ExerciseDto {
  id: number
  name: string
  targetRepsLow: number | null
  targetRepsHigh: number | null
  repsDisplay: string
  cue: string | null
  baseSets: number
  sortOrder: number
  video: VideoDto | null
}

export interface WorkoutDayDto {
  id: number
  name: string
  focus: string
  sortOrder: number
  exercises: ExerciseDto[]
}

export interface PhaseTodayDto {
  phaseNumber: number
  weekNumber: number
  repsHintText: string
  setDelta: number
  restWorkSeconds: number
  restBetweenSetsSeconds: number
  restBetweenExercisesSeconds: number
  notes: string | null
  programStartDate: string // ISO date (yyyy-MM-dd)
}

export interface WarmUpDto {
  name: string
  rounds: number
  secondsPerRound: number
  transitionSeconds: number
}

export interface SettingsDto {
  programStartDate: string
  daysPerWeekTarget: number
}

export interface StatsSummaryDto {
  totalSessions: number
  sessionsThisWeek: number
  sessionsThisMonth: number
  monthlyTarget: number
  currentWeeklyStreak: number
  averageSessionsPerWeek: number
  perDayCompletedCounts: Record<number, number>
}

export interface SessionDto {
  id: number
  workoutDayId: number
  workoutDayName: string
  date: string // yyyy-MM-dd
  phaseNumberAtCompletion: number
  startedAt: string
  completedAt: string | null
}

export interface CreateSessionRequest {
  workoutDayId: number
  date: string
  startedAt: string
  completedAt: string | null
}

export interface UpdateSessionRequest {
  workoutDayId: number
  date: string
  completedAt: string | null
}

// ---- Auth ----

export interface UserDto {
  id: number
  email: string
  displayName: string
  hasPassword: boolean
  googleLinked: boolean
}

export interface AuthResponse {
  token: string
  expiresAt: string
  user: UserDto
}

export interface AuthConfigDto {
  googleEnabled: boolean
  googleClientId: string | null
}

export interface RegisterRequest {
  email: string
  password: string
  displayName?: string
}

export interface LoginRequest {
  email: string
  password: string
}

// ---- Workout configuration (full CRUD) ----

export interface CreateWorkoutDayRequest {
  name: string
  focus?: string
}

export interface UpdateWorkoutDayRequest {
  name: string
  focus?: string
}

export interface UpsertExerciseRequest {
  name: string
  repsDisplay?: string
  targetRepsLow: number | null
  targetRepsHigh: number | null
  baseSets: number
  cue?: string | null
  videoId: number | null
}

export interface VideoLibraryItemDto {
  id: number
  title: string
  video: VideoDto
}
