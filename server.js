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

    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);

    // Security check to avoid directory traversal outside of PUBLIC_DIR
    if (!filePath.startsWith(PUBLIC_DIR)) {
        res.writeHead(403, { 'Content-Type': 'text/plain' });
        res.end('Forbidden');
        console.log(`\x1b[90m[${timestamp}]\x1b[0m \x1b[41m\x1b[37m 403 \x1b[0m ${req.method.padEnd(4)} \x1b[31m${reqPath}\x1b[0m`);
        return;
    }

    fs.access(filePath, fs.constants.F_OK, (err) => {
        if (err) {
            res.writeHead(404, { 'Content-Type': 'text/plain' });
            res.end('404 Not Found');
            console.log(`\x1b[90m[${timestamp}]\x1b[0m \x1b[43m\x1b[30m 404 \x1b[0m ${req.method.padEnd(4)} \x1b[33m${reqPath}\x1b[0m`);
            return;
        }

        fs.readFile(filePath, (readErr, data) => {
            if (readErr) {
                res.writeHead(500, { 'Content-Type': 'text/plain' });
                res.end('500 Internal Server Error');
                console.log(`\x1b[90m[${timestamp}]\x1b[0m \x1b[41m\x1b[37m 500 \x1b[0m ${req.method.padEnd(4)} \x1b[31m${reqPath}\x1b[0m`);
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
            
            // Beautiful console logs
            let statusColor = '\x1b[42m\x1b[30m'; // Default Green bg for OK
            let pathColor = '\x1b[37m';           // Default White
            
            if (ext === '.wasm' || ext === '.data') {
                statusColor = '\x1b[46m\x1b[30m'; // Cyan bg for heavy Unity files
                pathColor = '\x1b[36m';
            } else if (ext.match(/\.(png|jpg|gif|svg)$/)) {
                statusColor = '\x1b[45m\x1b[30m'; // Magenta bg for images
                pathColor = '\x1b[35m';
            } else if (ext === '.js' || ext === '.json') {
                statusColor = '\x1b[44m\x1b[37m'; // Blue bg for scripts
            }
            
            console.log(`\x1b[90m[${timestamp}]\x1b[0m ${statusColor} 200 \x1b[0m ${req.method.padEnd(4)} ${pathColor}${reqPath}\x1b[0m`);
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
        console.log('\n\x1b[36m=================================================================\x1b[0m');
        console.log('\x1b[1m\x1b[32m🚀  ULAR TANGGA IPB - LOCAL DEVELOPMENT SERVER IS LIVE  🚀\x1b[0m');
        console.log('\x1b[36m=================================================================\x1b[0m');
        console.log(`\n\x1b[1m🌍  Game URL:\x1b[0m        \x1b[4m\x1b[33mhttp://localhost:${PORT}\x1b[0m`);
        console.log(`\x1b[1m📂  Serving From:\x1b[0m    \x1b[37m${PUBLIC_DIR}\x1b[0m`);
        console.log(`\x1b[1m💡  Status:\x1b[0m          \x1b[42m\x1b[30m ONLINE \x1b[0m \x1b[32mReady for presentation\x1b[0m\n`);
        console.log('\x1b[90mPress Ctrl+C to stop the server. Live request logs will appear below:\x1b[0m');
        console.log('\x1b[36m-----------------------------------------------------------------\x1b[0m\n');
    });

    server.on('error', (e) => {
        if (e.code === 'EADDRINUSE') {
            console.error(`\n\x1b[41m\x1b[37m ERROR \x1b[0m \x1b[31mPort ${PORT} is still in use after cleanup.\x1b[0m`);
            console.error(`\x1b[33mTip: Try running: taskkill /F /IM node.exe\x1b[0m\n`);
            process.exit(1);
        } else {
            console.error(`\n\x1b[41m\x1b[37m FATAL \x1b[0m`, e);
            process.exit(1);
        }
    });
}, 500);
