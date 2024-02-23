const WebSocket = require('ws');
const http = require('http');
const express = require('express');

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

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

// Start the server
const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`);
});
