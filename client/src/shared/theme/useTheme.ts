import { create } from 'zustand'

export type ThemeName = 'neon' | 'light' | 'calm'

export const THEMES: { value: ThemeName; label: string }[] = [
  { value: 'neon', label: 'HUD / Neon' },
  { value: 'light', label: 'Classic Light' },
  { value: 'calm', label: 'Calm Teal' },
]

const STORAGE_KEY = 'ib_theme'

function load(): ThemeName {
  try {
    const v = localStorage.getItem(STORAGE_KEY)
    if (v === 'neon' || v === 'light' || v === 'calm') return v
  } catch {
    /* storage unavailable */
  }
  return 'neon'
}

function apply(theme: ThemeName) {
  document.documentElement.dataset.theme = theme
}

interface ThemeState {
  theme: ThemeName
  setTheme: (theme: ThemeName) => void
}

export const useTheme = create<ThemeState>((set) => ({
  theme: load(),
  setTheme: (theme) => {
    try {
      localStorage.setItem(STORAGE_KEY, theme)
    } catch {
      /* storage unavailable */
    }
    apply(theme)
    set({ theme })
  },
}))

// Apply the persisted theme as soon as this module is imported.
apply(load())
