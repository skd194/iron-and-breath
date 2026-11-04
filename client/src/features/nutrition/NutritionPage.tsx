import { useEffect, useState, type FormEvent } from 'react'
import {
  useCreateMeal,
  useDeleteMeal,
  useNutrition,
  useUpdateDietDay,
  useUpdateMeal,
  useUpdateNutritionProfile,
} from '../../shared/api/hooks'
import type { DietDayDto, MealDto, MealSlot, NutritionProfileDto } from '../../shared/api/types'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import './nutrition.css'

const WEEKDAY = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']
const SLOTS: MealSlot[] = ['Breakfast', 'Lunch', 'Snack', 'Dinner']

export function NutritionPage() {
  const nutritionQ = useNutrition()

  if (nutritionQ.isLoading) return <LoadingState label="Loading your nutrition plan…" />
  if (nutritionQ.isError || !nutritionQ.data) {
    return <ErrorState message="Couldn't load nutrition." onRetry={() => nutritionQ.refetch()} />
  }

  const { profile, days } = nutritionQ.data

  return (
    <div className="nutrition grid">
      <div>
        <h1 className="page-title">Nutrition</h1>
        <p className="muted">{profile.dietStyle} · muscle-building targets for your bodyweight.</p>
      </div>

      <ProfileCard profile={profile} />

      <section className="grid">
        <div className="section-title">Weekly plan</div>
        {days.map((day) => (
          <DietDayCard key={day.id} day={day} proteinTarget={profile.proteinTargetG} />
        ))}
      </section>

      <p className="nutrition-disclaimer muted">
        The weekly fasting day is a gentle one-meal day (plus fluids), not a hard fast — intended to
        rest the gut while keeping protein up. This is general guidance, not medical advice; check
        with a doctor if you have ongoing gut issues.
      </p>
    </div>
  )
}

function ProfileCard({ profile }: { profile: NutritionProfileDto }) {
  const update = useUpdateNutritionProfile()
  const [editing, setEditing] = useState(false)
  const [weight, setWeight] = useState(String(profile.weightKg))
  const [protein, setProtein] = useState(String(profile.proteinTargetG))
  const [calories, setCalories] = useState(String(profile.calorieTarget))
  const [style, setStyle] = useState(profile.dietStyle)
  const [notes, setNotes] = useState(profile.notes ?? '')

  useEffect(() => {
    setWeight(String(profile.weightKg))
    setProtein(String(profile.proteinTargetG))
    setCalories(String(profile.calorieTarget))
    setStyle(profile.dietStyle)
    setNotes(profile.notes ?? '')
  }, [profile])

  const save = (e: FormEvent) => {
    e.preventDefault()
    update.mutate(
      {
        weightKg: parseFloat(weight) || profile.weightKg,
        proteinTargetG: parseInt(protein, 10) || profile.proteinTargetG,
        calorieTarget: parseInt(calories, 10) || profile.calorieTarget,
        dietStyle: style.trim(),
        notes: notes.trim() || null,
      },
      { onSuccess: () => setEditing(false) },
    )
  }

  return (
    <section className="card card-pad">
      <div className="section-head-row">
        <div className="section-title" style={{ margin: 0 }}>Targets</div>
        <button className="btn btn-ghost btn-sm" onClick={() => setEditing((v) => !v)}>
          {editing ? 'Cancel' : 'Edit'}
        </button>
      </div>

      {editing ? (
        <form className="nutrition-profile-form" onSubmit={save}>
          <label className="field">Weight (kg)
            <input className="input" type="number" step="0.1" value={weight} onChange={(e) => setWeight(e.target.value)} />
          </label>
          <label className="field">Protein target (g)
            <input className="input" type="number" value={protein} onChange={(e) => setProtein(e.target.value)} />
          </label>
          <label className="field">Calories
            <input className="input" type="number" value={calories} onChange={(e) => setCalories(e.target.value)} />
          </label>
          <label className="field" style={{ gridColumn: '1 / -1' }}>Diet style
            <input className="input" value={style} onChange={(e) => setStyle(e.target.value)} />
          </label>
          <label className="field" style={{ gridColumn: '1 / -1' }}>Notes
            <input className="input" value={notes} onChange={(e) => setNotes(e.target.value)} />
          </label>
          <div className="nutrition-profile-actions">
            <button type="submit" className="btn btn-primary btn-sm" disabled={update.isPending}>
              {update.isPending ? 'Saving…' : 'Save'}
            </button>
          </div>
        </form>
      ) : (
        <div className="nutrition-targets">
          <Target label="Bodyweight" value={`${profile.weightKg} kg`} />
          <Target label="Protein / day" value={`${profile.proteinTargetG} g`} accent />
          <Target label="Calories / day" value={`${profile.calorieTarget}`} />
          {profile.notes && <p className="muted nutrition-notes">{profile.notes}</p>}
        </div>
      )}
    </section>
  )
}

function Target({ label, value, accent }: { label: string; value: string; accent?: boolean }) {
  return (
    <div className="nutrition-target">
      <div className={`nutrition-target-value${accent ? ' accent' : ''}`}>{value}</div>
      <div className="nutrition-target-label">{label}</div>
    </div>
  )
}

function DietDayCard({ day, proteinTarget }: { day: DietDayDto; proteinTarget: number }) {
  const updateDay = useUpdateDietDay()
  const [adding, setAdding] = useState(false)

  const pct = proteinTarget > 0 ? Math.min(1, day.totalProteinG / proteinTarget) : 0

  return (
    <div className={`card card-pad diet-day${day.isFastingDay ? ' fasting' : ''}`}>
      <div className="diet-day-head">
        <div className="diet-day-title">
          {WEEKDAY[day.dayOfWeek]}
          {day.isFastingDay && <span className="badge warn">Fasting</span>}
        </div>
        <div className="diet-day-protein">
          <span className="mono">{day.totalProteinG}g</span>
          <span className="muted"> / {proteinTarget}g protein</span>
        </div>
      </div>

      <div className="diet-protein-bar">
        <span className="diet-protein-fill" style={{ width: `${pct * 100}%` }} />
      </div>

      {day.notes && <p className="diet-day-notes muted">{day.notes}</p>}

      <div className="diet-meals">
        {day.meals.map((meal) => (
          <MealRow key={meal.id} dayId={day.id} meal={meal} />
        ))}
      </div>

      {adding ? (
        <MealForm dayId={day.id} onDone={() => setAdding(false)} />
      ) : (
        <div className="diet-day-actions">
          <button className="btn btn-ghost btn-sm" onClick={() => setAdding(true)}>+ Add meal</button>
          <label className="field-check diet-fasting-toggle">
            <input
              type="checkbox"
              checked={day.isFastingDay}
              onChange={(e) => updateDay.mutate({ id: day.id, body: { isFastingDay: e.target.checked, notes: day.notes } })}
            />
            Fasting day
          </label>
        </div>
      )}
    </div>
  )
}

function MealRow({ dayId, meal }: { dayId: number; meal: MealDto }) {
  const [editing, setEditing] = useState(false)
  const del = useDeleteMeal()

  if (editing) {
    return <MealForm dayId={dayId} meal={meal} onDone={() => setEditing(false)} />
  }

  return (
    <div className="meal-row">
      <div className="meal-slot">{meal.slot}</div>
      <div className="meal-body">
        <div className="meal-items">{meal.items}</div>
        <div className="meal-meta muted">
          {meal.timeHint && <span>{meal.timeHint}</span>}
          <span className="mono">{meal.proteinG}g</span>
        </div>
      </div>
      <div className="meal-actions">
        <button className="btn btn-ghost btn-sm" onClick={() => setEditing(true)}>Edit</button>
        <button className="btn btn-danger btn-sm" disabled={del.isPending}
          onClick={() => del.mutate({ dayId, mealId: meal.id })}>×</button>
      </div>
    </div>
  )
}

function MealForm({ dayId, meal, onDone }: { dayId: number; meal?: MealDto; onDone: () => void }) {
  const create = useCreateMeal()
  const update = useUpdateMeal()
  const [slot, setSlot] = useState<MealSlot>(meal?.slot ?? 'Breakfast')
  const [timeHint, setTimeHint] = useState(meal?.timeHint ?? '')
  const [items, setItems] = useState(meal?.items ?? '')
  const [protein, setProtein] = useState(String(meal?.proteinG ?? 0))
  const pending = create.isPending || update.isPending

  const submit = (e: FormEvent) => {
    e.preventDefault()
    if (!items.trim()) return
    const body = { slot, timeHint: timeHint.trim() || null, items: items.trim(), proteinG: parseInt(protein, 10) || 0 }
    if (meal) update.mutate({ dayId, mealId: meal.id, body }, { onSuccess: onDone })
    else create.mutate({ dayId, body }, { onSuccess: onDone })
  }

  return (
    <form className="meal-form" onSubmit={submit}>
      <div className="meal-form-row">
        <select className="input" value={slot} onChange={(e) => setSlot(e.target.value as MealSlot)}>
          {SLOTS.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
        <input className="input" placeholder="Time" value={timeHint} onChange={(e) => setTimeHint(e.target.value)} />
        <input className="input meal-protein-input" type="number" placeholder="g" value={protein} onChange={(e) => setProtein(e.target.value)} />
      </div>
      <input className="input" placeholder="Foods…" value={items} onChange={(e) => setItems(e.target.value)} autoFocus required />
      <div className="meal-form-actions">
        <button type="button" className="btn btn-ghost btn-sm" onClick={onDone}>Cancel</button>
        <button type="submit" className="btn btn-primary btn-sm" disabled={pending || !items.trim()}>
          {pending ? 'Saving…' : meal ? 'Save' : 'Add'}
        </button>
      </div>
    </form>
  )
}
