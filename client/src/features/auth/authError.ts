import { ApiError } from '../../shared/api/client'

/** Best-effort human message from an API error body. */
export function authErrorMessage(err: unknown, fallback: string): string {
  if (err instanceof ApiError) {
    const body = err.body as { message?: string; title?: string; errors?: Record<string, string[]> } | undefined
    if (body?.message) return body.message
    if (body?.errors) {
      const first = Object.values(body.errors)[0]?.[0]
      if (first) return first
    }
    if (body?.title) return body.title
  }
  return fallback
}
