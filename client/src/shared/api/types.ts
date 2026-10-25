// Mirrors the API DTOs (IronAndBreath.Api/Dtos).

export interface VideoDto {
  embedUrl: string
  thumbnailUrl: string | null
  title: string
  durationSeconds: number | null
  attribution: string | null
}

export interface BreathingDto {
  concentric: string | null
  eccentric: string | null
  notes: string | null
}

/** Structured per-rep tempo (seconds) that drives the work-screen metronome. */
export interface TempoDto {
  concentricSeconds: number | null
  eccentricSeconds: number | null
  pauseSeconds: number | null
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
  primaryMuscles: string[]
  secondaryMuscles: string[]
  breathing: BreathingDto | null
  tempo: string | null
  tempoSpec: TempoDto | null
  benefits: string | null
  commonMistakes: string | null
  safetyTips: string | null
  animationRef: string | null
}

export interface WorkoutDayDto {
  id: number
  name: string
  focus: string
  sortOrder: number
  /** Weekday this day is scheduled on: 0=Sun..6=Sat, or null = unscheduled. */
  scheduledDayOfWeek: number | null
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

export type WorkoutSource = 'Guided' | 'Manual' | 'Imported'

export interface SetLogDto {
  id: number
  exerciseId: number | null
  exerciseName: string | null
  setNumber: number
  repsCompleted: number | null
  weightKg: number | null
  rpe: number | null
  notes: string | null
}

export interface SessionDto {
  id: number
  workoutDayId: number | null
  workoutDayName: string | null
  date: string // yyyy-MM-dd
  phaseNumberAtCompletion: number
  startedAt: string
  completedAt: string | null
  source: WorkoutSource
  notes: string | null
  perceivedDifficulty: number | null
  setLogs: SetLogDto[]
}

export interface CreateSetLogRequest {
  exerciseId?: number | null
  exerciseName?: string | null
  setNumber: number
  repsCompleted?: number | null
  weightKg?: number | null
  rpe?: number | null
  notes?: string | null
}

export interface CreateSessionRequest {
  workoutDayId: number | null
  date: string
  startedAt: string
  completedAt: string | null
  source?: WorkoutSource
  notes?: string | null
  perceivedDifficulty?: number | null
  setLogs?: CreateSetLogRequest[]
}

export interface UpdateSessionRequest {
  workoutDayId: number | null
  date: string
  completedAt: string | null
  notes?: string | null
  perceivedDifficulty?: number | null
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
  scheduledDayOfWeek?: number | null
}

export interface UpdateWorkoutDayRequest {
  name: string
  focus?: string
  scheduledDayOfWeek?: number | null
}

export interface UpsertExerciseRequest {
  name: string
  repsDisplay?: string
  targetRepsLow: number | null
  targetRepsHigh: number | null
  baseSets: number
  cue?: string | null
  videoId: number | null
  tempoConcentricSeconds?: number | null
  tempoEccentricSeconds?: number | null
  tempoPauseSeconds?: number | null
  breathingConcentric?: string | null
  breathingEccentric?: string | null
  breathingNotes?: string | null
  primaryMuscles?: string[]
  secondaryMuscles?: string[]
  tempo?: string | null
  benefits?: string | null
  commonMistakes?: string | null
  safetyTips?: string | null
  animationRef?: string | null
}

export interface VideoLibraryItemDto {
  id: number
  title: string
  video: VideoDto
}

// ---- AI coach ----

export interface CoachMessageDto {
  id: number
  role: 'user' | 'assistant'
  content: string
  createdAt: string
}

export interface CoachStateDto {
  aiEnabled: boolean
  provider: string
  messages: CoachMessageDto[]
}

// ---- Equipment inventory ----

export type EquipmentCategory =
  | 'Bench'
  | 'Barbell'
  | 'Dumbbell'
  | 'Plates'
  | 'Rack'
  | 'Resistance'
  | 'Cardio'
  | 'Bodyweight'
  | 'Accessory'
  | 'Other'

export type EquipmentStatus = 'Owned' | 'Planned'

export interface EquipmentItemDto {
  id: number
  name: string
  category: EquipmentCategory
  specs: string | null
  status: EquipmentStatus
  sortOrder: number
}

export interface UpsertEquipmentRequest {
  name: string
  category: EquipmentCategory
  specs?: string | null
  status: EquipmentStatus
}

// ---- Muscle reference catalog ----

export interface MuscleDto {
  id: number
  name: string
  group: string
  function: string
  injuryNotes: string | null
}

// ---- Nutrition ----

export type MealSlot = 'Breakfast' | 'Lunch' | 'Snack' | 'Dinner'

export interface NutritionProfileDto {
  weightKg: number
  proteinTargetG: number
  calorieTarget: number
  dietStyle: string
  notes: string | null
}

export interface UpdateNutritionProfileRequest {
  weightKg: number
  proteinTargetG: number
  calorieTarget: number
  dietStyle: string
  notes?: string | null
}

export interface MealDto {
  id: number
  slot: MealSlot
  timeHint: string | null
  items: string
  proteinG: number
  sortOrder: number
}

export interface DietDayDto {
  id: number
  /** 0=Sun..6=Sat */
  dayOfWeek: number
  isFastingDay: boolean
  notes: string | null
  totalProteinG: number
  meals: MealDto[]
}

export interface NutritionDto {
  profile: NutritionProfileDto
  days: DietDayDto[]
}

export interface UpdateDietDayRequest {
  isFastingDay: boolean
  notes?: string | null
}

export interface UpsertMealRequest {
  slot: MealSlot
  timeHint?: string | null
  items: string
  proteinG: number
}

// ---- Bodyweight tracking ----

export interface BodyMetricDto {
  id: number
  date: string // yyyy-MM-dd
  weightKg: number
}

export interface LogBodyMetricRequest {
  date: string
  weightKg: number
}
