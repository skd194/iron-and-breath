import type { PhaseTodayDto, VideoDto, WarmUpDto, WorkoutDayDto } from '../../shared/api/types'

export type StepKind =
  | 'warmup-round'
  | 'warmup-transition'
  | 'work'
  | 'rest-set'
  | 'rest-exercise'

export interface SessionStep {
  id: string
  kind: StepKind
  title: string
  subtitle?: string
  durationSeconds: number
  isRest: boolean
  // Work-step context
  exerciseName?: string
  repsDisplay?: string
  cue?: string | null
  video?: VideoDto | null
  setNumber?: number
  totalSets?: number
  // Warm-up context
  roundNumber?: number
  totalRounds?: number
}

/** Sets performed for an exercise under a given phase (baseSets + phase delta). */
export function setsForExercise(baseSets: number, phase: PhaseTodayDto): number {
  return Math.max(1, baseSets + phase.setDelta)
}

export type AgendaGroupKind = 'warmup' | 'exercise'

/** One collapsible block in the plan list: the warm-up, or a single exercise. */
export interface AgendaGroup {
  key: string
  kind: AgendaGroupKind
  title: string
  /** Step index range this group spans (inclusive), for progress + jumping. */
  startIndex: number
  endIndex: number
  totalSeconds: number
  // Exercise context
  reps?: string
  cue?: string | null
  sets?: number
  workSeconds?: number
  restSeconds?: number
  // Warm-up context
  rounds?: number
  roundSeconds?: number
  transitionSeconds?: number
}

/**
 * Folds the flat step list into exercise-level groups (plus one warm-up group)
 * so the plan can be shown as cards with reps/rest details instead of every
 * individual timer step. Pure; derives all timing from the steps themselves.
 */
export function groupStepsForAgenda(steps: SessionStep[]): AgendaGroup[] {
  const groups: AgendaGroup[] = []
  let cur: AgendaGroup | null = null
  let closed = false

  for (let i = 0; i < steps.length; i++) {
    const s = steps[i]
    if (s.kind === 'warmup-round' || s.kind === 'warmup-transition') {
      if (!cur || cur.kind !== 'warmup') {
        cur = { key: `warmup-${i}`, kind: 'warmup', title: 'Warm-up', startIndex: i, endIndex: i, totalSeconds: 0 }
        groups.push(cur)
      }
      cur.endIndex = i
      cur.totalSeconds += s.durationSeconds
      if (s.kind === 'warmup-round') {
        cur.title = s.title
        cur.rounds = (cur.rounds ?? 0) + 1
        cur.roundSeconds = s.durationSeconds
      } else {
        cur.transitionSeconds = s.durationSeconds
      }
    } else if (s.kind === 'work') {
      if (!cur || cur.kind !== 'exercise' || closed) {
        cur = {
          key: `ex-${i}`,
          kind: 'exercise',
          title: s.exerciseName ?? s.title,
          startIndex: i,
          endIndex: i,
          totalSeconds: 0,
          reps: s.repsDisplay,
          cue: s.cue,
          sets: s.totalSets,
          workSeconds: s.durationSeconds,
        }
        groups.push(cur)
        closed = false
      }
      cur.endIndex = i
      cur.totalSeconds += s.durationSeconds
    } else if (s.kind === 'rest-set') {
      if (cur) {
        cur.endIndex = i
        cur.restSeconds = s.durationSeconds
        cur.totalSeconds += s.durationSeconds
      }
    } else if (s.kind === 'rest-exercise') {
      if (cur) {
        cur.endIndex = i
        cur.totalSeconds += s.durationSeconds
        closed = true // trailing rest ends this exercise; next work starts a new group
      }
    }
  }

  return groups
}

/**
 * Builds the full scripted step sequence for a session: warm-up rounds, then
 * each exercise's work sets with rests between sets and between exercises.
 * Pure and deterministic so it can be reasoned about and (later) tested.
 */
export function buildSessionPlan(
  day: WorkoutDayDto,
  phase: PhaseTodayDto,
  warmup: WarmUpDto,
): SessionStep[] {
  const steps: SessionStep[] = []
  let n = 0
  const push = (s: Omit<SessionStep, 'id'>) => steps.push({ ...s, id: `${s.kind}-${n++}` })

  // --- Warm-up: N rounds of Surya Namaskar with transition breaks between ---
  for (let r = 1; r <= warmup.rounds; r++) {
    push({
      kind: 'warmup-round',
      title: warmup.name,
      subtitle: `Round ${r} of ${warmup.rounds}`,
      durationSeconds: warmup.secondsPerRound,
      isRest: false,
      roundNumber: r,
      totalRounds: warmup.rounds,
    })
    if (r < warmup.rounds) {
      push({
        kind: 'warmup-transition',
        title: 'Breathe',
        subtitle: 'Transition',
        durationSeconds: warmup.transitionSeconds,
        isRest: true,
      })
    }
  }

  // --- Exercises ---
  const exercises = [...day.exercises].sort((a, b) => a.sortOrder - b.sortOrder)
  exercises.forEach((ex, ei) => {
    const sets = setsForExercise(ex.baseSets, phase)
    for (let s = 1; s <= sets; s++) {
      push({
        kind: 'work',
        title: ex.name,
        subtitle: `Set ${s} of ${sets}`,
        durationSeconds: phase.restWorkSeconds,
        isRest: false,
        exerciseName: ex.name,
        repsDisplay: ex.repsDisplay,
        cue: ex.cue,
        video: ex.video,
        setNumber: s,
        totalSets: sets,
      })
      if (s < sets) {
        push({
          kind: 'rest-set',
          title: 'Rest',
          subtitle: `Next: ${ex.name} · set ${s + 1}`,
          durationSeconds: phase.restBetweenSetsSeconds,
          isRest: true,
        })
      }
    }
    if (ei < exercises.length - 1) {
      const next = exercises[ei + 1]
      push({
        kind: 'rest-exercise',
        title: 'Rest',
        subtitle: `Next up: ${next.name}`,
        durationSeconds: phase.restBetweenExercisesSeconds,
        isRest: true,
      })
    }
  })

  return steps
}
