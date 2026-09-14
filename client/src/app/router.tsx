import type { ReactNode } from 'react'
import { createBrowserRouter, Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../features/auth/AuthContext'
import { LoginPage } from '../features/auth/LoginPage'
import { RegisterPage } from '../features/auth/RegisterPage'
import { CoachPage } from '../features/coach/CoachPage'
import { ConfigPage } from '../features/config/ConfigPage'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { HistoryPage } from '../features/history/HistoryPage'
import { ManualLogPage } from '../features/manual-log/ManualLogPage'
import { SessionPlayerPage } from '../features/session-player/SessionPlayerPage'
import { LoadingState } from '../shared/ui/States'
import { AppLayout } from './AppLayout'

function RequireAuth({ children }: { children: ReactNode }) {
  const { isAuthenticated, loading, user } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }
  // Token present but the user hasn't resolved yet: show a spinner briefly.
  if (loading && !user) {
    return <LoadingState label="Loading your account…" />
  }
  return <>{children}</>
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/register', element: <RegisterPage /> },
  {
    path: '/',
    element: (
      <RequireAuth>
        <AppLayout />
      </RequireAuth>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'coach', element: <CoachPage /> },
      { path: 'history', element: <HistoryPage /> },
      { path: 'log', element: <ManualLogPage /> },
      { path: 'settings', element: <ConfigPage /> },
    ],
  },
  // The session player runs full-screen without the app chrome.
  {
    path: '/session/:dayId',
    element: (
      <RequireAuth>
        <SessionPlayerPage />
      </RequireAuth>
    ),
  },
])
