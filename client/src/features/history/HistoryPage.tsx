import { useState } from 'react'
import { useDeleteSession, useSessions, useWorkoutDays } from '../../shared/api/hooks'
import type { SessionDto } from '../../shared/api/types'
import { EmptyState, ErrorState, LoadingState } from '../../shared/ui/States'
import { formatLong } from '../../shared/util/date'
import { EditSessionModal } from './EditSessionModal'
import './history.css'

export function HistoryPage() {
  const sessionsQ = useSessions()
  const daysQ = useWorkoutDays()
  const deleteSession = useDeleteSession()

  const [editing, setEditing] = useState<SessionDto | null>(null)
  const [pendingDelete, setPendingDelete] = useState<SessionDto | null>(null)

  if (sessionsQ.isLoading || daysQ.isLoading) {
    return <LoadingState label="Loading history…" />
  }
  if (sessionsQ.isError || !sessionsQ.data) {
    return <ErrorState message="Couldn't load history." onRetry={() => sessionsQ.refetch()} />
  }

  const sessions = sessionsQ.data
  const days = daysQ.data ?? []

  return (
    <div className="history">
      <div className="history-head">
        <h1>History</h1>
        <span className="muted">{sessions.length} logged</span>
      </div>

      {sessions.length === 0 ? (
        <div className="card card-pad">
          <EmptyState title="No sessions yet">
            Complete a workout from the dashboard and it'll show up here.
          </EmptyState>
        </div>
      ) : (
        <div className="card">
          <ul className="session-list">
            {sessions.map((s) => (
              <li key={s.id} className="session-row">
                <div className="session-main">
                  <span className="session-date">{formatLong(s.date)}</span>
                  <div className="session-tags">
                    <span className="badge badge-accent">{s.workoutDayName}</span>
                    <span className="badge">Phase {s.phaseNumberAtCompletion}</span>
                    <span className={`badge ${s.completedAt ? 'ok' : 'warn'}`}>
                      {s.completedAt ? '✓ Completed' : 'Abandoned'}
                    </span>
                  </div>
                </div>
                <div className="session-actions">
                  <button className="btn btn-ghost" onClick={() => setEditing(s)}>
                    Edit
                  </button>
                  <button className="btn btn-danger" onClick={() => setPendingDelete(s)}>
                    Delete
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {editing && (
        <EditSessionModal
          session={editing}
          days={days}
          onClose={() => setEditing(null)}
        />
      )}

      {pendingDelete && (
        <div className="modal-overlay" onClick={() => setPendingDelete(null)}>
          <div className="modal card card-pad" onClick={(e) => e.stopPropagation()}>
            <h2>Delete session?</h2>
            <p className="muted">
              {formatLong(pendingDelete.date)} · {pendingDelete.workoutDayName}. This can't be undone.
            </p>
            <div className="modal-actions">
              <button className="btn btn-ghost" onClick={() => setPendingDelete(null)}>
                Cancel
              </button>
              <button
                className="btn btn-danger"
                disabled={deleteSession.isPending}
                onClick={() =>
                  deleteSession.mutate(pendingDelete.id, {
                    onSuccess: () => setPendingDelete(null),
                  })
                }
              >
                {deleteSession.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
