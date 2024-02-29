const WebSocket = require('ws');

function createServer(httpServer) {
    const wss = new WebSocket.Server({ server: httpServer });
    const clients = new Set();

    wss.on('connection', (ws) => {
        clients.add(ws);
        console.log('Client connected to ShopServer');

        ws.on('message', (message) => {
            console.log("Received raw message in ShopServer:", message);
            // Tu lógica de manejo de mensajes de la tienda aquí
        });
        
        ws.on('close', () => {
            clients.delete(ws);
            console.log('Client disconnected from ShopServer');
        });
    });

    return wss;
}

module.exports = { createServer };
