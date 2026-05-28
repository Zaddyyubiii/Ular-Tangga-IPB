import os
import sys
import http.server
import socketserver

PORT = 3000
PUBLIC_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'docs')

class WebGLHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        # Add CORS and caching headers for reliable local loads
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Cache-Control', 'no-cache, no-store, must-revalidate')
        self.send_header('Pragma', 'no-cache')
        self.send_header('Expires', '0')
        super().end_headers()

    def translate_path(self, path):
        # Always serve from the 'docs' subdirectory
        path = super().translate_path(path)
        rel = os.path.relpath(path, os.getcwd())
        return os.path.join(PUBLIC_DIR, rel)

# Register custom MIME types for WebGL compatibility
extensions_map = http.server.SimpleHTTPRequestHandler.extensions_map
extensions_map.update({
    '.wasm': 'application/wasm',
    '.data': 'application/octet-stream',
    '.js': 'application/javascript',
    '.css': 'text/css',
    '.html': 'text/html',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
})

class ThreadingHTTPServer(socketserver.ThreadingMixIn, http.server.HTTPServer):
    daemon_threads = True

def run():
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    server_address = ('', PORT)
    
    try:
        with ThreadingHTTPServer(server_address, WebGLHandler) as httpd:
            print("\x1b[32m==================================================\x1b[0m")
            print("\x1b[36m  Ular Tangga IPB WebGL Local Server (Python)\x1b[0m")
            print("\x1b[32m==================================================\x1b[0m")
            print(f"Server is running at: http://localhost:{PORT}")
            print(f"Serving files from: {PUBLIC_DIR}")
            print("Press Ctrl+C to stop the server.")
            print("--------------------------------------------------")
            httpd.serve_forever()
    except KeyboardInterrupt:
        print("\nStopping server...")
        sys.exit(0)
    except Exception as e:
        print(f"Error starting server: {e}")
        sys.exit(1)

if __name__ == '__main__':
    run()
