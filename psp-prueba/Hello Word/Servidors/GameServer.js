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
            const jsonData = JSON.parse(message);
            console.log("Received JSON data:", jsonData);

            // Example: Broadcasting the received score data to all clients
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

// Handle upgrade for WebSocket
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

// Función para enviar puntajes a todos los clientes
function broadcastScore(data) {
    const jsonData = JSON.stringify(data);
    clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(jsonData);
        }
    });
}

wss.broadcastScore = broadcastScore;
