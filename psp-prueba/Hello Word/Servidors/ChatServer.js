const WebSocket = require('ws');

const wss = new WebSocket.Server({ noServer: true });

// Store connected clients
const clients = new Set();

wss.on('connection', (ws) => {
    clients.add(ws);
    console.log('Client connected');

    ws.on('message', (message) => {
        console.log("Received raw message:", message);
    
        try {
            const parsedMessage = JSON.parse(message);
            console.log("Parsed message:", parsedMessage);
    
            // Broadcast the received message to all clients
            wss.clients.forEach((client) => {
                if (client.readyState === WebSocket.OPEN) {
                    console.log("Broadcasting message:", parsedMessage);
                    client.send(JSON.stringify(parsedMessage));
                }
            });
        } catch (error) {
            console.error("Error parsing message:", error);
        }
    });
    
    ws.on('close', () => {
        clients.delete(ws);
        console.log('Client disconnected');
    });
});

// Handle upgrade for WebSocket
const server = require('http').createServer();
server.on('upgrade', (request, socket, head) => {
    wss.handleUpgrade(request, socket, head, (ws) => {
        wss.emit('connection', ws, request);
    });
});

const PORT = 3001;
server.listen(PORT, () => {
    console.log(`Child WebSocket server running on port ${PORT}`);
});
