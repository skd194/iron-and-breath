import { api } from '../../shared/api/client'
import { getToken, setToken } from '../../shared/api/token'
import type { CoachStateDto } from '../../shared/api/types'

const BASE = import.meta.env.VITE_API_BASE ?? ''

export function getCoach(): Promise<CoachStateDto> {
  return api.get<CoachStateDto>('/api/coach')
}

export function resetCoach(): Promise<void> {
  return api.del<void>('/api/coach')
}

interface StreamHandlers {
  onDelta: (text: string) => void
  onError?: (message: string) => void
  signal?: AbortSignal
}

/**
 * Sends a message and consumes the assistant reply as Server-Sent Events.
 * Uses fetch + a ReadableStream reader (not EventSource) so the JWT can travel
 * on the Authorization header. Parses `data: {json}\n\n` frames emitted by the API.
 */
export async function streamCoachMessage(content: string, handlers: StreamHandlers): Promise<void> {
  const token = getToken()
  const res = await fetch(`${BASE}/api/coach/messages`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify({ content }),
    signal: handlers.signal,
  })

  if (res.status === 401) {
    setToken(null)
    throw new Error('Your session expired. Please sign in again.')
  }
  if (!res.ok || !res.body) {
    throw new Error('Could not reach the coach.')
  }

  const reader = res.body.getReader()
  const decoder = new TextDecoder()
  let buffer = ''

  for (;;) {
    const { done, value } = await reader.read()
    if (done) break
    buffer += decoder.decode(value, { stream: true })

    let sep: number
    while ((sep = buffer.indexOf('\n\n')) !== -1) {
      const frame = buffer.slice(0, sep).trim()
      buffer = buffer.slice(sep + 2)
      if (!frame.startsWith('data:')) continue
      const json = frame.slice(5).trim()
      if (!json) continue
      try {
        const evt = JSON.parse(json) as { type: string; text?: string; message?: string }
        if (evt.type === 'delta' && evt.text) handlers.onDelta(evt.text)
        else if (evt.type === 'error') handlers.onError?.(evt.message ?? 'The coach hit an error.')
      } catch {
        // ignore malformed frame
      }
    }
  }
}
