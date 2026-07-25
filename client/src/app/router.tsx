import { createBrowserRouter } from 'react-router-dom'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { HistoryPage } from '../features/history/HistoryPage'
import { SessionPlayerPage } from '../features/session-player/SessionPlayerPage'
import { AppLayout } from './AppLayout'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'history', element: <HistoryPage /> },
    ],
  },
  // The session player runs full-screen without the app chrome.
  { path: '/session/:dayId', element: <SessionPlayerPage /> },
])
