import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const currentDir = path.dirname(fileURLToPath(import.meta.url))

const unityAssetTypes = {
  '.data': 'application/octet-stream',
  '.js': 'text/javascript; charset=utf-8',
  '.wasm': 'application/wasm',
  '.json': 'application/json; charset=utf-8',
  '.png': 'image/png',
  '.jpg': 'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.ico': 'image/x-icon',
  '.css': 'text/css; charset=utf-8'
}

function unityBuildDevServer() {
  const docsDir = path.resolve(currentDir, '../docs')
  const roots = ['/Build', '/TemplateData', '/StreamingAssets']

  return {
    name: 'unity-build-dev-server',
    configureServer(server) {
      server.middlewares.use((req, res, next) => {
        const requestUrl = decodeURIComponent((req.url || '').split('?')[0])
        const root = roots.find(prefix => requestUrl === prefix || requestUrl.startsWith(`${prefix}/`))

        if (!root) {
          next()
          return
        }

        const relativePath = requestUrl.replace(/^\/+/, '')
        const filePath = path.resolve(docsDir, relativePath)

        if (!filePath.startsWith(docsDir) || !fs.existsSync(filePath) || !fs.statSync(filePath).isFile()) {
          next()
          return
        }

        const ext = path.extname(filePath)
        res.statusCode = 200
        res.setHeader('Content-Type', unityAssetTypes[ext] || 'application/octet-stream')
        fs.createReadStream(filePath).pipe(res)
      })
    }
  }
}

// https://vite.dev/config/
export default defineConfig({
  base: './', // CRITICAL: Forces relative paths so compiled assets load flawlessly on GitHub Pages subpaths!
  plugins: [
    unityBuildDevServer(),
    react(),
    tailwindcss()
  ],
  build: {
    outDir: '../docs',
    emptyOutDir: false, // Prevents wiping out Unity WebGL build files (Build/ and TemplateData/)
  }
})


