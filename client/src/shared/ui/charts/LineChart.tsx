import './charts.css'

export interface LinePoint {
  label: string
  value: number
}

interface LineChartProps {
  points: LinePoint[]
  height?: number
  color?: string
  unit?: string
  /** Minimum span for the y-axis so tiny changes aren't exaggerated. */
  minSpan?: number
}

/**
 * A minimal, dependency-free SVG line chart with a soft area fill. Scales to the
 * data's min/max (with padding) and labels the first/last points and the y-range.
 */
export function LineChart({ points, height = 160, color = 'var(--accent, var(--breath-500))', unit = '', minSpan = 1 }: LineChartProps) {
  if (points.length === 0) {
    return <div className="chart-empty muted">No data yet.</div>
  }
  if (points.length === 1) {
    return (
      <div className="chart-single">
        <span className="chart-single-value mono">{points[0].value}{unit}</span>
        <span className="muted">{points[0].label}</span>
      </div>
    )
  }

  const W = 100 // viewBox width units; SVG scales to container
  const H = 100
  const pad = 6
  const values = points.map((p) => p.value)
  let min = Math.min(...values)
  let max = Math.max(...values)
  if (max - min < minSpan) {
    const mid = (max + min) / 2
    min = mid - minSpan / 2
    max = mid + minSpan / 2
  }
  const span = max - min || 1

  const x = (i: number) => pad + (i / (points.length - 1)) * (W - pad * 2)
  const y = (v: number) => pad + (1 - (v - min) / span) * (H - pad * 2)

  const line = points.map((p, i) => `${x(i).toFixed(2)},${y(p.value).toFixed(2)}`).join(' ')
  const area = `${pad},${H - pad} ${line} ${(W - pad).toFixed(2)},${H - pad}`

  return (
    <div className="chart" style={{ height }}>
      <svg viewBox={`0 0 ${W} ${H}`} preserveAspectRatio="none" className="chart-svg" role="img">
        <polygon points={area} fill={color} opacity={0.12} />
        <polyline
          points={line}
          fill="none"
          stroke={color}
          strokeWidth={1.4}
          strokeLinejoin="round"
          strokeLinecap="round"
          vectorEffect="non-scaling-stroke"
        />
        {points.map((p, i) => (
          <circle key={i} cx={x(i)} cy={y(p.value)} r={1.1} fill={color} />
        ))}
      </svg>
      <div className="chart-axis">
        <span className="muted">{points[0].label}</span>
        <span className="chart-range mono">
          {Math.round(min)}–{Math.round(max)}{unit}
        </span>
        <span className="muted">{points[points.length - 1].label}</span>
      </div>
    </div>
  )
}
