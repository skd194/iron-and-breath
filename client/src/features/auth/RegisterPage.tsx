import { useCallback, useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from './AuthContext'
import { GoogleButton } from './GoogleButton'
import { authErrorMessage } from './authError'
import './auth.css'

export function RegisterPage() {
  const { register, loginWithGoogle, googleClientId, isAuthenticated } = useAuth()
  const navigate = useNavigate()

  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  if (isAuthenticated) {
    return <Navigate to="/" replace />
  }

  const submit = async (e: FormEvent) => {
    e.preventDefault()
    setError(null)
    if (password.length < 8) {
      setError('Password must be at least 8 characters.')
      return
    }
    setBusy(true)
    try {
      await register({ email, password, displayName: displayName || undefined })
      navigate('/', { replace: true })
    } catch (err) {
      setError(authErrorMessage(err, 'Could not create your account.'))
    } finally {
      setBusy(false)
    }
  }

  const onGoogle = useCallback(
    async (idToken: string) => {
      setError(null)
      try {
        await loginWithGoogle(idToken)
        navigate('/', { replace: true })
      } catch (err) {
        setError(authErrorMessage(err, 'Google sign-in failed.'))
      }
    },
    [loginWithGoogle, navigate],
  )

  return (
    <div className="auth-shell">
      <div className="card card-pad auth-card">
        <div className="auth-brand">
          Iron <span className="amp">&amp;</span> Breath
        </div>
        <h1 className="auth-title">Create your account</h1>
        <p className="muted auth-sub">Your own program, sessions, and progress.</p>

        <form className="auth-form" onSubmit={submit}>
          <label className="field">
            Name <span className="muted">(optional)</span>
            <input
              type="text"
              autoComplete="name"
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
            />
          </label>
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
            Password <span className="muted">(min 8 characters)</span>
            <input
              type="password"
              autoComplete="new-password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </label>

          {error && <div className="auth-error">{error}</div>}

          <button className="btn btn-primary btn-lg" type="submit" disabled={busy}>
            {busy ? 'Creating…' : 'Create account'}
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
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </div>
    </div>
  )
}
