const http = require('http');
const fs = require('fs');
const path = require('path');

const PUBLIC_DIR = path.join(__dirname, 'docs');
const PORT = parseInt(process.env.PORT, 10) || 3000;

const MIME_TYPES = {
    '.html': 'text/html',
    '.css': 'text/css',
    '.js': 'application/javascript',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon',
    '.wasm': 'application/wasm',
    '.data': 'application/octet-stream',
    '.br': 'application/octet-stream',
    '.gz': 'application/octet-stream'
};

const server = http.createServer((req, res) => {
    // Clean URL query or hash params
    let reqPath = req.url.split('?')[0].split('#')[0];

    // Default to index.html if root is requested
    if (reqPath === '/' || reqPath === '') {
        reqPath = '/index.html';
    }

    const filePath = path.join(PUBLIC_DIR, reqPath);

    // Security check to avoid directory traversal outside of PUBLIC_DIR
    if (!filePath.startsWith(PUBLIC_DIR)) {
        res.writeHead(403, { 'Content-Type': 'text/plain' });
        res.end('Forbidden');
        return;
    }

    fs.access(filePath, fs.constants.F_OK, (err) => {
        if (err) {
            res.writeHead(404, { 'Content-Type': 'text/plain' });
            res.end('404 Not Found');
            return;
        }

        fs.readFile(filePath, (readErr, data) => {
            if (readErr) {
                res.writeHead(500, { 'Content-Type': 'text/plain' });
                res.end('500 Internal Server Error');
                return;
            }

            const ext = path.extname(filePath).toLowerCase();
            const contentType = MIME_TYPES[ext] || 'application/octet-stream';

            res.writeHead(200, {
                'Content-Type': contentType,
                'Access-Control-Allow-Origin': '*',
                'X-Content-Type-Options': 'nosniff',
                'Cache-Control': 'no-store, no-cache, must-revalidate, proxy-revalidate',
                'Pragma': 'no-cache',
                'Expires': '0'
            });
            res.end(data);
        });
    });
});

// Kill whatever is on the port first, then listen
const { execSync } = require('child_process');

function killPort(port) {
    try {
        // Windows: find PID on the port and kill it
        const result = execSync(
            `netstat -ano | findstr :${port} | findstr LISTENING`,
            { encoding: 'utf-8', timeout: 3000 }
        );
        const lines = result.trim().split('\n');
        const pids = new Set();
        for (const line of lines) {
            const parts = line.trim().split(/\s+/);
            const pid = parts[parts.length - 1];
            if (pid && pid !== '0' && !isNaN(pid)) {
                pids.add(pid);
            }
        }
        for (const pid of pids) {
            try {
                execSync(`taskkill /F /PID ${pid}`, { timeout: 3000 });
                console.log(`[Port] Killed stale process PID ${pid} on port ${port}`);
            } catch { /* already dead */ }
        }
    } catch {
        // Nothing on that port — good
    }
}

killPort(PORT);

// Small delay to let OS release the port
setTimeout(() => {
    server.listen(PORT, () => {
        console.log('\x1b[32m%s\x1b[0m', '==================================================');
        console.log('\x1b[36m%s\x1b[0m', '  Ular Tangga IPB WebGL Local Server (Node.js)');
        console.log('\x1b[32m%s\x1b[0m', '==================================================');
        console.log(`Server is running at: \x1b[33mhttp://localhost:${PORT}\x1b[0m`);
        console.log(`Serving files from: ${PUBLIC_DIR}`);
        console.log('Press Ctrl+C to stop the server.');
        console.log('--------------------------------------------------');
    });

    server.on('error', (e) => {
        if (e.code === 'EADDRINUSE') {
            console.error(`\x1b[31m[Error] Port ${PORT} still in use after cleanup. Try: taskkill /F /IM node.exe\x1b[0m`);
            process.exit(1);
        } else {
            console.error(e);
            process.exit(1);
        }
    });
}, 500);
