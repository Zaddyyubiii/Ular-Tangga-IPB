const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3000;
const PUBLIC_DIR = path.join(__dirname, 'docs');

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
    '.data': 'application/octet-stream'
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
            // File not found
            res.writeHead(404, { 'Content-Type': 'text/plain' });
            res.end('404 Not Found');
            return;
        }

        fs.readFile(filePath, (readErr, data) => {
            if (readErr) {
                // Internal server error
                res.writeHead(500, { 'Content-Type': 'text/plain' });
                res.end('500 Internal Server Error');
                return;
            }

            // Get MIME type based on file extension
            const ext = path.extname(filePath).toLowerCase();
            const contentType = MIME_TYPES[ext] || 'application/octet-stream';

            // Add security and CORS headers
            res.writeHead(200, {
                'Content-Type': contentType,
                'Access-Control-Allow-Origin': '*',
                'X-Content-Type-Options': 'nosniff',
                'Cache-Control': 'no-cache'
            });
            res.end(data);
        });
    });
});

server.listen(PORT, () => {
    console.log('\x1b[32m%s\x1b[0m', '==================================================');
    console.log('\x1b[36m%s\x1b[0m', '  Ular Tangga IPB WebGL Local Server (Node.js)');
    console.log('\x1b[32m%s\x1b[0m', '==================================================');
    console.log(`Server is running at: http://localhost:${PORT}`);
    console.log(`Serving files from: ${PUBLIC_DIR}`);
    console.log('Press Ctrl+C to stop the server.');
    console.log('--------------------------------------------------');
});
