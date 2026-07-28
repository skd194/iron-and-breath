import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../features/auth/AuthContext'

export function AppLayout() {
  const { user, logout } = useAuth()

  const initial = (user?.displayName || user?.email || '?').charAt(0).toUpperCase()

  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="container topbar-inner">
          <NavLink to="/" className="brand">
            Iron <span className="amp">&amp;</span> Breath
          </NavLink>
          <nav className="nav">
            <NavLink to="/" end>
              Dashboard
            </NavLink>
            <NavLink to="/history">History</NavLink>
            <NavLink to="/settings">Configure</NavLink>
          </nav>
          <div className="user-menu">
            <span className="user-chip" title={user?.email}>
              <span className="user-avatar" aria-hidden>
                {initial}
              </span>
              <span className="user-name">{user?.displayName || user?.email}</span>
            </span>
            <button className="btn btn-ghost btn-sm" onClick={logout}>
              Sign out
            </button>
          </div>
        </div>
      </header>
      <main>
        <div className="container">
          <Outlet />
        </div>
      </main>
    </div>
  )
}
