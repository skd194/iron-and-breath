// Tiny observable store for the JWT. Kept out of React so the api client can
// read it synchronously, and components can subscribe via useSyncExternalStore.
const KEY = 'ib_token'

let current: string | null = readInitial()
const listeners = new Set<() => void>()

function readInitial(): string | null {
  try {
    return localStorage.getItem(KEY)
  } catch {
    return null
  }
}

export function getToken(): string | null {
  return current
}

export function setToken(token: string | null): void {
  current = token
  try {
    if (token) localStorage.setItem(KEY, token)
    else localStorage.removeItem(KEY)
  } catch {
    /* ignore storage failures (private mode, etc.) */
  }
  listeners.forEach((l) => l())
}

export function subscribeToken(listener: () => void): () => void {
  listeners.add(listener)
  return () => {
    listeners.delete(listener)
  }
}
