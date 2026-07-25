import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// The API runs on http://localhost:5201 in development (HTTPS redirect is
// disabled there). We proxy /api so the client can use same-origin relative
// URLs and avoid CORS entirely during dev.
const API_TARGET = process.env.VITE_API_TARGET ?? 'http://localhost:5201'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: API_TARGET,
        changeOrigin: true,
      },
    },
  },
})
