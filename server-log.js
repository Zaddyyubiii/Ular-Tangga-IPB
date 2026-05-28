const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3001;
let clients = [];

const server = http.createServer((req, res) => {
    // Enable CORS for all requests
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type');

    if (req.method === 'OPTIONS') {
        res.writeHead(200);
        res.end();
        return;
    }

    if (req.url === '/log' && req.method === 'POST') {
        let body = '';
        req.on('data', chunk => { body += chunk; });
        req.on('end', () => {
            try {
                const logData = JSON.parse(body);
                // Broadcast to all connected SSE clients
                clients.forEach(client => {
                    client.write(`data: ${JSON.stringify(logData)}\n\n`);
                });
                res.writeHead(200, { 'Content-Type': 'text/plain' });
                res.end('OK');
            } catch (err) {
                res.writeHead(400, { 'Content-Type': 'text/plain' });
                res.end('Bad Request');
            }
        });
        return;
    }

    if (req.url === '/events') {
        res.writeHead(200, {
            'Content-Type': 'text/event-stream',
            'Cache-Control': 'no-cache',
            'Connection': 'keep-alive'
        });
        
        clients.push(res);
        
        req.on('close', () => {
            clients = clients.filter(client => client !== res);
        });
        return;
    }

    // Serve a beautiful dark-themed dashboard at GET /
    if (req.url === '/' || req.url === '/index.html') {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end(`
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Ular Tangga IPB - Remote Log Inspector</title>
    <link href="https://fonts.googleapis.com/css2?family=Outfit:wght@400;600;800;900&display=swap" rel="stylesheet">
    <style>
        body {
            margin: 0;
            padding: 20px;
            background-color: #0b0d19;
            color: #e2e8f0;
            font-family: 'Outfit', sans-serif;
            display: flex;
            flex-direction: column;
            height: 100vh;
            box-sizing: border-box;
        }
        header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 2px solid #ff0055;
            padding-bottom: 10px;
            margin-bottom: 15px;
        }
        h1 {
            margin: 0;
            font-size: 24px;
            font-weight: 800;
            color: #ff0055;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .stats {
            font-size: 12px;
            background: rgba(255,255,255,0.05);
            padding: 4px 10px;
            border-radius: 20px;
            border: 1px solid rgba(255,255,255,0.1);
        }
        .controls {
            display: flex;
            gap: 10px;
            margin-bottom: 15px;
            align-items: center;
        }
        button, input {
            background: #1e293b;
            border: 2px solid rgba(255,255,255,0.1);
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-weight: 600;
            font-family: inherit;
            cursor: pointer;
            transition: all 0.2s;
        }
        button:hover {
            background: #ff0055;
            border-color: #ff0055;
            box-shadow: 0 0 10px rgba(255,0,85,0.4);
        }
        input {
            flex-grow: 1;
            cursor: text;
        }
        #console {
            background: #060810;
            border: 3px solid rgba(255,255,255,0.05);
            border-radius: 16px;
            padding: 15px;
            flex-grow: 1;
            overflow-y: auto;
            font-family: monospace;
            font-size: 12px;
            line-height: 1.6;
            box-shadow: inset 0 4px 20px rgba(0,0,0,0.8);
        }
        .log-entry {
            border-bottom: 1px solid rgba(255,255,255,0.05);
            padding: 6px 0;
            animation: fadeIn 0.15s ease-out;
            display: flex;
            gap: 10px;
        }
        .time {
            color: #64748b;
            flex-shrink: 0;
        }
        .type {
            font-weight: bold;
            padding: 0 4px;
            border-radius: 4px;
            font-size: 10px;
            text-align: center;
            width: 50px;
            flex-shrink: 0;
        }
        .type-log { background: rgba(16,185,129,0.15); color: #10b981; }
        .type-warn { background: rgba(245,158,11,0.15); color: #f59e0b; }
        .type-err { background: rgba(239,68,68,0.15); color: #ef4444; }
        .type-crash { background: rgba(244,63,94,0.25); color: #f43f5e; border: 1px solid #f43f5e; }
        .msg {
            white-space: pre-wrap;
            word-break: break-all;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(4px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>
</head>
<body>
    <header>
        <h1>🐞 REMOTE LOG INSPECTOR (Port 3001)</h1>
        <div class="stats" id="client-count">Listening for events...</div>
    </header>
    <div class="controls">
        <button id="clear-btn">Clear Logs</button>
        <button id="scroll-btn">Autoscroll: ON</button>
        <input type="text" id="filter-input" placeholder="Search and filter logs...">
    </div>
    <div id="console">
        <div class="log-entry">
            <span class="time">[System]</span>
            <span class="type type-log">SYS</span>
            <span class="msg" style="color: #64748b;">Connected to Event Stream. Listening for logs from localhost:3000...</span>
        </div>
    </div>

    <script>
        const consoleEl = document.getElementById('console');
        const clearBtn = document.getElementById('clear-btn');
        const scrollBtn = document.getElementById('scroll-btn');
        const filterInput = document.getElementById('filter-input');

        let autoscroll = true;
        let logs = [];

        // SSE connection
        const source = new EventSource('/events');

        source.onmessage = function(event) {
            const data = JSON.parse(event.data);
            logs.push(data);
            renderLog(data);
        };

        source.onerror = function() {
            const entry = {
                type: 'SYS',
                message: 'Connection to log stream lost. Reconnecting...',
                timestamp: new Date().toLocaleTimeString()
            };
            renderLog(entry);
        };

        function renderLog(data) {
            const entryDiv = document.createElement('div');
            entryDiv.className = 'log-entry';
            
            const timeSpan = document.createElement('span');
            timeSpan.className = 'time';
            timeSpan.innerText = '[' + (data.timestamp || new Date().toLocaleTimeString()) + ']';

            const typeSpan = document.createElement('span');
            typeSpan.className = 'type type-' + data.type.toLowerCase();
            typeSpan.innerText = data.type;

            const msgSpan = document.createElement('span');
            msgSpan.className = 'msg';
            msgSpan.innerText = data.message;

            entryDiv.appendChild(timeSpan);
            entryDiv.appendChild(typeSpan);
            entryDiv.appendChild(msgSpan);

            // Filter check
            const filterText = filterInput.value.toLowerCase();
            if (filterText && !data.message.toLowerCase().includes(filterText) && !data.type.toLowerCase().includes(filterText)) {
                entryDiv.style.display = 'none';
            }

            consoleEl.appendChild(entryDiv);

            if (autoscroll) {
                consoleEl.scrollTop = consoleEl.scrollHeight;
            }
        }

        clearBtn.onclick = function() {
            consoleEl.innerHTML = '';
            logs = [];
        };

        scrollBtn.onclick = function() {
            autoscroll = !autoscroll;
            scrollBtn.innerText = 'Autoscroll: ' + (autoscroll ? 'ON' : 'OFF');
        };

        filterInput.oninput = function() {
            const filterText = filterInput.value.toLowerCase();
            const children = consoleEl.children;
            for (let i = 0; i < children.length; i++) {
                const row = children[i];
                const msg = row.querySelector('.msg');
                const type = row.querySelector('.type');
                if (!msg || !type) continue;
                
                const textMatch = msg.innerText.toLowerCase().includes(filterText) || type.innerText.toLowerCase().includes(filterText);
                row.style.display = textMatch ? 'flex' : 'none';
            }
        };
    </script>
</body>
</html>
        `);
        return;
    }

    res.writeHead(404);
    res.end();
});

server.listen(PORT, () => {
    console.log('\x1b[32m%s\x1b[0m', '==================================================');
    console.log('\x1b[36m%s\x1b[0m', '  Ular Tangga IPB - Remote Log Server (Node.js)');
    console.log('\x1b[32m%s\x1b[0m', '==================================================');
    console.log(`Remote inspector is running at: http://localhost:${PORT}`);
    console.log(`Viewing stream at: http://localhost:${PORT}`);
    console.log('Press Ctrl+C to stop the log server.');
    console.log('--------------------------------------------------');
});
