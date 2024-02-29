const WebSocket = require('ws');

const wss = new WebSocket.Server({ noServer: true });

const clients = new Set();

wss.on('connection', (ws) => {
    clients.add(ws);
    console.log('Client connected');

    ws.on('message', (message) => {
        console.log("Received raw message:", message);

        try {
            const jsonData = JSON.parse(message);
            console.log("Received JSON data:", jsonData);

            wss.broadcastScore(jsonData);
        } catch (error) {
            console.error("Error parsing JSON:", error);
        }
    });
    
    ws.on('close', () => {
        clients.delete(ws);
        console.log('Client disconnected');
    });
});

const server = require('http').createServer();
server.on('upgrade', (request, socket, head) => {
    wss.handleUpgrade(request, socket, head, (ws) => {
        wss.emit('connection', ws, request);
    });
});

const PORT = 3002;
server.listen(PORT, () => {
    console.log(`Child WebSocket server running on port ${PORT}`);
});


function broadcastScore(data) {
    const jsonData = JSON.stringify(data);
    clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(jsonData);
        }
    });
}

wss.broadcastScore = broadcastScore;
