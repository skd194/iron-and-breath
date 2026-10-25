import './charts.css'

export interface Bar {
  label: string
  value: number
  /** Optional override colour for this bar. */
  color?: string
}

interface BarChartProps {
  bars: Bar[]
  height?: number
  color?: string
  /** Show the numeric value above each bar. */
  showValues?: boolean
}

/** A minimal, dependency-free CSS bar chart (flex columns scaled to the max). */
export function BarChart({ bars, height = 160, color = 'var(--accent, var(--breath-500))', showValues = true }: BarChartProps) {
  if (bars.length === 0) {
    return <div className="chart-empty muted">No data yet.</div>
  }
  const max = Math.max(1, ...bars.map((b) => b.value))

  return (
    <div className="barchart" style={{ height }}>
      {bars.map((b, i) => (
        <div key={i} className="barchart-col" title={`${b.label}: ${b.value}`}>
          <div className="barchart-track">
            {showValues && b.value > 0 && <span className="barchart-value mono">{b.value}</span>}
            <div
              className="barchart-bar"
              style={{ height: `${(b.value / max) * 100}%`, background: b.color ?? color }}
            />
          </div>
          <span className="barchart-label">{b.label}</span>
        </div>
      ))}
    </div>
  )
}
