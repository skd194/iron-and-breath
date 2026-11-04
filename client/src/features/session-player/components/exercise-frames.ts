// Movement demonstration frames from the free-exercise-db (CC0), served via the
// jsDelivr CDN. Each exercise has two frames (start + end of the movement) which
// the player alternates to animate the rep. Keyed by the exercise's animationRef
// slug (see SeedData.Slug on the server). Add entries here to cover more movements.

const CDN = 'https://cdn.jsdelivr.net/gh/yuhonas/free-exercise-db@main/exercises/'

const FRAMES: Record<string, string[]> = {
  'flat-barbell-bench-press': ['Barbell_Bench_Press_-_Medium_Grip/0.jpg', 'Barbell_Bench_Press_-_Medium_Grip/1.jpg'],
  'incline-dumbbell-press': ['Incline_Dumbbell_Press/0.jpg', 'Incline_Dumbbell_Press/1.jpg'],
  'incline-dumbbell-flye': ['Incline_Dumbbell_Flyes/0.jpg', 'Incline_Dumbbell_Flyes/1.jpg'],
  'seated-dumbbell-shoulder-press': ['Dumbbell_Shoulder_Press/0.jpg', 'Dumbbell_Shoulder_Press/1.jpg'],
  'lateral-raise': ['Side_Lateral_Raise/0.jpg', 'Side_Lateral_Raise/1.jpg'],
  'overhead-triceps-extension': ['Standing_Overhead_Barbell_Triceps_Extension/0.jpg', 'Standing_Overhead_Barbell_Triceps_Extension/1.jpg'],
  'push-ups': ['Pushups/0.jpg', 'Pushups/1.jpg'],
  'barbell-bent-over-row': ['Bent_Over_Barbell_Row/0.jpg', 'Bent_Over_Barbell_Row/1.jpg'],
  'one-arm-dumbbell-row': ['One-Arm_Dumbbell_Row/0.jpg', 'One-Arm_Dumbbell_Row/1.jpg'],
  'pull-ups': ['Pullups/0.jpg', 'Pullups/1.jpg'],
  'barbell-curl': ['Barbell_Curl/0.jpg', 'Barbell_Curl/1.jpg'],
  'hammer-curl': ['Hammer_Curls/0.jpg', 'Hammer_Curls/1.jpg'],
  'rear-delt-flye': ['Reverse_Flyes/0.jpg', 'Reverse_Flyes/1.jpg'],
  'superman-back-extension-hold': ['Superman/0.jpg', 'Superman/1.jpg'],
  'barbell-back-squat': ['Barbell_Full_Squat/0.jpg', 'Barbell_Full_Squat/1.jpg'],
  'romanian-deadlift': ['Romanian_Deadlift/0.jpg', 'Romanian_Deadlift/1.jpg'],
  'bulgarian-split-squat': ['Split_Squat_with_Dumbbells/0.jpg', 'Split_Squat_with_Dumbbells/1.jpg'],
  'walking-lunge': ['Barbell_Walking_Lunge/0.jpg', 'Barbell_Walking_Lunge/1.jpg'],
  'standing-calf-raise': ['Standing_Calf_Raises/0.jpg', 'Standing_Calf_Raises/1.jpg'],
  'plank-hold': ['Plank/0.jpg', 'Plank/1.jpg'],
  'hanging-leg-raise': ['Hanging_Leg_Raise/0.jpg', 'Hanging_Leg_Raise/1.jpg'],
  'dumbbell-thruster': ['Kettlebell_Thruster/0.jpg', 'Kettlebell_Thruster/1.jpg'],
  'dumbbell-deadlift': ['Barbell_Deadlift/0.jpg', 'Barbell_Deadlift/1.jpg'],
  'renegade-row': ['Alternating_Renegade_Row/0.jpg', 'Alternating_Renegade_Row/1.jpg'],
  'russian-twist': ['Russian_Twist/0.jpg', 'Russian_Twist/1.jpg'],
}

/** Full CDN frame URLs for an exercise's animationRef slug, or null if none. */
export function framesFor(ref?: string | null): string[] | null {
  if (!ref) return null
  const frames = FRAMES[ref]
  return frames && frames.length > 0 ? frames.map((p) => CDN + p) : null
}
