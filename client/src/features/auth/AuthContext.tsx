import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  useSyncExternalStore,
  type ReactNode,
} from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { api } from '../../shared/api/client'
import { getToken, setToken, subscribeToken } from '../../shared/api/token'
import type {
  AuthConfigDto,
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserDto,
} from '../../shared/api/types'

interface AuthState {
  user: UserDto | null
  token: string | null
  isAuthenticated: boolean
  loading: boolean
  googleClientId: string | null
  login: (body: LoginRequest) => Promise<void>
  register: (body: RegisterRequest) => Promise<void>
  loginWithGoogle: (idToken: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthState | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const qc = useQueryClient()
  const token = useSyncExternalStore(subscribeToken, getToken, getToken)
  const [user, setUser] = useState<UserDto | null>(null)
  const [loading, setLoading] = useState<boolean>(() => !!getToken())
  const [googleClientId, setGoogleClientId] = useState<string | null>(null)

  // Public bootstrap: which providers are enabled (for the Google button).
  useEffect(() => {
    api
      .get<AuthConfigDto>('/api/auth/config')
      .then((c) => setGoogleClientId(c.googleEnabled ? c.googleClientId : null))
      .catch(() => setGoogleClientId(null))
  }, [])

  // Resolve the current user whenever the token changes.
  useEffect(() => {
    let cancelled = false
    if (!token) {
      setUser(null)
      setLoading(false)
      return
    }
    setLoading(true)
    api
      .get<UserDto>('/api/auth/me')
      .then((u) => !cancelled && setUser(u))
      .catch(() => {
        if (!cancelled) {
          setUser(null)
          setToken(null)
        }
      })
      .finally(() => !cancelled && setLoading(false))
    return () => {
      cancelled = true
    }
  }, [token])

  const applyAuth = useCallback((res: AuthResponse) => {
    setUser(res.user)
    setToken(res.token)
  }, [])

  const login = useCallback(
    async (body: LoginRequest) => applyAuth(await api.post<AuthResponse>('/api/auth/login', body)),
    [applyAuth],
  )
  const register = useCallback(
    async (body: RegisterRequest) => applyAuth(await api.post<AuthResponse>('/api/auth/register', body)),
    [applyAuth],
  )
  const loginWithGoogle = useCallback(
    async (idToken: string) => applyAuth(await api.post<AuthResponse>('/api/auth/google', { idToken })),
    [applyAuth],
  )
  const logout = useCallback(() => {
    setToken(null)
    setUser(null)
    qc.clear()
  }, [qc])

  const value = useMemo<AuthState>(
    () => ({
      user,
      token,
      isAuthenticated: !!token,
      loading,
      googleClientId,
      login,
      register,
      loginWithGoogle,
      logout,
    }),
    [user, token, loading, googleClientId, login, register, loginWithGoogle, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthState {
  const ctx = useContext(AuthContext)
  if (!ctx) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return ctx
}
