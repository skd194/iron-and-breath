import { useEffect, useRef } from 'react'

interface GisId {
  initialize: (config: {
    client_id: string
    callback: (response: { credential: string }) => void
  }) => void
  renderButton: (parent: HTMLElement, options: Record<string, unknown>) => void
}

declare global {
  interface Window {
    google?: { accounts: { id: GisId } }
  }
}

const SRC = 'https://accounts.google.com/gsi/client'

function loadGis(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (window.google?.accounts?.id) return resolve()
    const existing = document.querySelector<HTMLScriptElement>(`script[src="${SRC}"]`)
    if (existing) {
      existing.addEventListener('load', () => resolve())
      existing.addEventListener('error', () => reject(new Error('gis')))
      return
    }
    const s = document.createElement('script')
    s.src = SRC
    s.async = true
    s.defer = true
    s.onload = () => resolve()
    s.onerror = () => reject(new Error('gis'))
    document.head.appendChild(s)
  })
}

/** Renders the official Google Identity Services button and returns its ID token. */
export function GoogleButton({
  clientId,
  onCredential,
}: {
  clientId: string
  onCredential: (idToken: string) => void
}) {
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    let cancelled = false
    loadGis()
      .then(() => {
        if (cancelled || !ref.current || !window.google) return
        window.google.accounts.id.initialize({
          client_id: clientId,
          callback: (r) => onCredential(r.credential),
        })
        window.google.accounts.id.renderButton(ref.current, {
          theme: 'filled_black',
          size: 'large',
          shape: 'pill',
          text: 'continue_with',
          width: 300,
        })
      })
      .catch(() => {
        /* network/blocked — the email/password form still works */
      })
    return () => {
      cancelled = true
    }
  }, [clientId, onCredential])

  return <div ref={ref} className="google-btn" aria-label="Continue with Google" />
}
