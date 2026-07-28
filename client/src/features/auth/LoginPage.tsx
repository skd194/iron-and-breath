import { useCallback, useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from './AuthContext'
import { GoogleButton } from './GoogleButton'
import { authErrorMessage } from './authError'
import './auth.css'

export function LoginPage() {
  const { login, loginWithGoogle, googleClientId, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const from = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/'

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  // Already signed in? Bounce to the app.
  if (isAuthenticated) {
    return <Navigate to={from} replace />
  }

  const submit = async (e: FormEvent) => {
    e.preventDefault()
    setError(null)
    setBusy(true)
    try {
      await login({ email, password })
      navigate(from, { replace: true })
    } catch (err) {
      setError(authErrorMessage(err, 'Could not sign in.'))
    } finally {
      setBusy(false)
    }
  }

  const onGoogle = useCallback(
    async (idToken: string) => {
      setError(null)
      try {
        await loginWithGoogle(idToken)
        navigate(from, { replace: true })
      } catch (err) {
        setError(authErrorMessage(err, 'Google sign-in failed.'))
      }
    },
    [loginWithGoogle, navigate, from],
  )

  return (
    <div className="auth-shell">
      <div className="card card-pad auth-card">
        <div className="auth-brand">
          Iron <span className="amp">&amp;</span> Breath
        </div>
        <h1 className="auth-title">Welcome back</h1>
        <p className="muted auth-sub">Sign in to continue your program.</p>

        <form className="auth-form" onSubmit={submit}>
          <label className="field">
            Email
            <input
              type="email"
              autoComplete="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </label>
          <label className="field">
            Password
            <input
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </label>

          {error && <div className="auth-error">{error}</div>}

          <button className="btn btn-primary btn-lg" type="submit" disabled={busy}>
            {busy ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

        {googleClientId && (
          <>
            <div className="auth-divider">
              <span>or</span>
            </div>
            <GoogleButton clientId={googleClientId} onCredential={onGoogle} />
          </>
        )}

        <p className="auth-alt muted">
          New here? <Link to="/register">Create an account</Link>
        </p>
      </div>
    </div>
  )
}
