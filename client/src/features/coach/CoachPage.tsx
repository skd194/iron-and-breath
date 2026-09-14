import { useEffect, useRef, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { LoadingState } from '../../shared/ui/States'
import { getCoach, resetCoach, streamCoachMessage } from './coachApi'
import './coach.css'

interface Msg {
  role: 'user' | 'assistant'
  content: string
}

const SUGGESTIONS = [
  'I want to build muscle and train 4 days a week',
  'Design a beginner full-body plan with dumbbells only',
  'How should I warm up before lifting?',
]

export function CoachPage() {
  const [messages, setMessages] = useState<Msg[]>([])
  const [provider, setProvider] = useState('')
  const [aiEnabled, setAiEnabled] = useState(true)
  const [loading, setLoading] = useState(true)
  const [input, setInput] = useState('')
  const [streaming, setStreaming] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const scrollRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    getCoach()
      .then((s) => {
        setMessages(s.messages.map((m) => ({ role: m.role, content: m.content })))
        setProvider(s.provider)
        setAiEnabled(s.aiEnabled)
      })
      .catch(() => setError('Could not load your coach.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    scrollRef.current?.scrollTo({ top: scrollRef.current.scrollHeight, behavior: 'smooth' })
  }, [messages])

  const send = async (text: string) => {
    const content = text.trim()
    if (!content || streaming) return
    setError(null)
    setInput('')
    // Optimistically add the user's message + an empty assistant bubble to fill in.
    setMessages((prev) => [...prev, { role: 'user', content }, { role: 'assistant', content: '' }])
    setStreaming(true)
    try {
      await streamCoachMessage(content, {
        onDelta: (delta) =>
          setMessages((prev) => {
            const next = [...prev]
            next[next.length - 1] = {
              role: 'assistant',
              content: next[next.length - 1].content + delta,
            }
            return next
          }),
        onError: (msg) => setError(msg),
      })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'The coach is unavailable.')
    } finally {
      setStreaming(false)
    }
  }

  const submit = (e: FormEvent) => {
    e.preventDefault()
    void send(input)
  }

  const onReset = async () => {
    await resetCoach().catch(() => undefined)
    setMessages([])
    setError(null)
  }

  if (loading) return <LoadingState label="Waking up your coach…" />

  const empty = messages.length === 0

  return (
    <div className="coach">
      <div className="coach-head">
        <div>
          <h1>AI Coach</h1>
          <span className="muted coach-provider">
            {provider}
            {!aiEnabled && ' · set CLAUDE_API_KEY for the full coach'}
          </span>
        </div>
        {!empty && (
          <button className="btn btn-ghost btn-sm" onClick={onReset} disabled={streaming}>
            New chat
          </button>
        )}
      </div>

      <div className="coach-thread" ref={scrollRef}>
        {empty ? (
          <div className="coach-welcome">
            <div className="coach-welcome-icon" aria-hidden>💪</div>
            <h2>Let's build your plan</h2>
            <p className="muted">
              Tell me your goals and I'll help you design a workout. Then set it up on{' '}
              <Link to="/settings">Configure</Link> and start a session from the dashboard.
            </p>
            <div className="coach-suggestions">
              {SUGGESTIONS.map((s) => (
                <button key={s} className="coach-suggestion" onClick={() => void send(s)}>
                  {s}
                </button>
              ))}
            </div>
          </div>
        ) : (
          messages.map((m, i) => (
            <div key={i} className={`coach-msg ${m.role}`}>
              <div className="coach-bubble">
                {m.content || (streaming && i === messages.length - 1 ? <span className="coach-typing">●●●</span> : '')}
              </div>
            </div>
          ))
        )}
      </div>

      {error && <div className="auth-error coach-error">{error}</div>}

      <form className="coach-input" onSubmit={submit}>
        <input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Ask your coach anything…"
          disabled={streaming}
          autoFocus
        />
        <button type="submit" className="btn btn-primary" disabled={streaming || !input.trim()}>
          {streaming ? '…' : 'Send'}
        </button>
      </form>
    </div>
  )
}
