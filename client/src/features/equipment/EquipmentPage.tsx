import { useState, type FormEvent } from 'react'
import {
  useCreateEquipment,
  useDeleteEquipment,
  useEquipment,
  useUpdateEquipment,
} from '../../shared/api/hooks'
import type { EquipmentCategory, EquipmentItemDto, EquipmentStatus } from '../../shared/api/types'
import { ErrorState, LoadingState } from '../../shared/ui/States'
import './equipment.css'

const CATEGORIES: EquipmentCategory[] = [
  'Bench', 'Barbell', 'Dumbbell', 'Plates', 'Rack', 'Resistance', 'Cardio', 'Bodyweight', 'Accessory', 'Other',
]

export function EquipmentPage() {
  const equipmentQ = useEquipment()
  const [adding, setAdding] = useState(false)

  return (
    <div className="equipment grid">
      <div className="section-head-row">
        <div>
          <h1 className="page-title">Equipment</h1>
          <p className="muted">What you train with. The coach uses this to tailor advice.</p>
        </div>
        <button className="btn btn-primary btn-sm" onClick={() => setAdding((v) => !v)}>
          {adding ? 'Close' : '+ Add equipment'}
        </button>
      </div>

      {adding && <EquipmentForm onDone={() => setAdding(false)} />}

      {equipmentQ.isLoading ? (
        <LoadingState label="Loading your equipment…" />
      ) : equipmentQ.isError || !equipmentQ.data ? (
        <ErrorState message="Couldn't load equipment." onRetry={() => equipmentQ.refetch()} />
      ) : (
        <EquipmentSections items={equipmentQ.data} />
      )}
    </div>
  )
}

function EquipmentSections({ items }: { items: EquipmentItemDto[] }) {
  const owned = items.filter((i) => i.status === 'Owned')
  const planned = items.filter((i) => i.status === 'Planned')
  return (
    <>
      <Section title="Owned" items={owned} empty="No equipment yet — add your first item." />
      {planned.length > 0 && <Section title="Planned" items={planned} empty="" />}
    </>
  )
}

function Section({ title, items, empty }: { title: string; items: EquipmentItemDto[]; empty: string }) {
  return (
    <section className="grid">
      <div className="section-title">{title}</div>
      {items.length === 0 ? (
        empty ? <p className="muted">{empty}</p> : null
      ) : (
        <div className="equip-grid">
          {items.map((item) => (
            <EquipmentCard key={item.id} item={item} />
          ))}
        </div>
      )}
    </section>
  )
}

function EquipmentCard({ item }: { item: EquipmentItemDto }) {
  const [editing, setEditing] = useState(false)
  const del = useDeleteEquipment()

  if (editing) {
    return <EquipmentForm item={item} onDone={() => setEditing(false)} />
  }

  return (
    <div className="card card-pad equip-card">
      <div className="equip-card-head">
        <span className={`equip-cat equip-cat-${item.category.toLowerCase()}`}>{item.category}</span>
        {item.status === 'Planned' && <span className="badge">Planned</span>}
      </div>
      <div className="equip-name">{item.name}</div>
      {item.specs && <div className="equip-specs muted">{item.specs}</div>}
      <div className="equip-actions">
        <button className="btn btn-ghost btn-sm" onClick={() => setEditing(true)}>
          Edit
        </button>
        <button
          className="btn btn-danger btn-sm"
          disabled={del.isPending}
          onClick={() => del.mutate(item.id)}
        >
          Delete
        </button>
      </div>
    </div>
  )
}

function EquipmentForm({ item, onDone }: { item?: EquipmentItemDto; onDone: () => void }) {
  const create = useCreateEquipment()
  const update = useUpdateEquipment()
  const [name, setName] = useState(item?.name ?? '')
  const [category, setCategory] = useState<EquipmentCategory>(item?.category ?? 'Other')
  const [specs, setSpecs] = useState(item?.specs ?? '')
  const [status, setStatus] = useState<EquipmentStatus>(item?.status ?? 'Owned')

  const pending = create.isPending || update.isPending

  const submit = (e: FormEvent) => {
    e.preventDefault()
    if (!name.trim()) return
    const body = { name: name.trim(), category, specs: specs.trim() || null, status }
    if (item) {
      update.mutate({ id: item.id, body }, { onSuccess: onDone })
    } else {
      create.mutate(body, { onSuccess: onDone })
    }
  }

  return (
    <form className="card card-pad equip-form" onSubmit={submit}>
      <div className="equip-form-row">
        <label className="field" style={{ flex: 2 }}>
          Name
          <input value={name} onChange={(e) => setName(e.target.value)} className="input" autoFocus required />
        </label>
        <label className="field">
          Category
          <select className="input" value={category} onChange={(e) => setCategory(e.target.value as EquipmentCategory)}>
            {CATEGORIES.map((c) => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        </label>
        <label className="field">
          Status
          <select className="input" value={status} onChange={(e) => setStatus(e.target.value as EquipmentStatus)}>
            <option value="Owned">Owned</option>
            <option value="Planned">Planned</option>
          </select>
        </label>
      </div>
      <label className="field">
        Specs (optional)
        <input value={specs} onChange={(e) => setSpecs(e.target.value)} className="input" placeholder="e.g. 4×2.5kg + 4×2kg plates" />
      </label>
      <div className="equip-form-actions">
        <button type="button" className="btn btn-ghost btn-sm" onClick={onDone}>
          Cancel
        </button>
        <button type="submit" className="btn btn-primary btn-sm" disabled={pending || !name.trim()}>
          {pending ? 'Saving…' : item ? 'Save' : 'Add'}
        </button>
      </div>
    </form>
  )
}
