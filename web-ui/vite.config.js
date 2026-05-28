import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  base: './', // CRITICAL: Forces relative paths so compiled assets load flawlessly on GitHub Pages subpaths!
  plugins: [
    react(),
    tailwindcss()
  ],
  build: {
    outDir: '../docs',
    emptyOutDir: false, // Prevents wiping out Unity WebGL build files (Build/ and TemplateData/)
  }
})


