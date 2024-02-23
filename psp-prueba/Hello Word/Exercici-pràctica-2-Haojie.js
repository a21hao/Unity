const http = require('http');
const WebSocket = require('ws');

const PUERTO = 3000;
let nextUserId = 1;
const usuarios = {};

const server = http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'application/json' }); // Cambiado a application/json
    res.end(JSON.stringify({ message: 'Servidor WebSocket' })); // Enviando un mensaje JSON
});

const wss = new WebSocket.Server({ server });

wss.on('connection', (ws) => {
    const userId = nextUserId++;
    console.log(`Cliente ${userId} conectado`);

    const initialMessage = {
        message: 'Por favor, introduce tu nombre de usuario o deja el campo en blanco para usar tu ID:'
    };
    ws.send(JSON.stringify(initialMessage)); // Enviando un mensaje JSON al cliente

    ws.on('message', (mensaje) => {
        let parsedMessage;
        try {
            parsedMessage = JSON.parse(mensaje);
        } catch (error) {
            console.error('Error al analizar el mensaje como JSON:', error);
            return;
        }
    
        if (!usuarios[userId]) {
            usuarios[userId] = parsedMessage.username || `Usuario_${userId}`;
            const userMessage = {
                message: `Nombre de usuario establecido como: ${usuarios[userId]}`
            };
            ws.send(JSON.stringify(userMessage)); // Enviando un mensaje JSON al cliente
            return;
        }

        console.log(`Mensaje recibido de ${usuarios[userId]} (${userId}): ${parsedMessage.message}`);

        const jsonMessage = {
            username: usuarios[userId],
            message: parsedMessage.message
        };

        wss.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify(jsonMessage)); // Enviando un mensaje JSON a todos los clientes
            }
        });
    });

    ws.on('close', () => {
        console.log(`Cliente ${userId} (${usuarios[userId]}) desconectado`);
        delete usuarios[userId];
    });
});

server.listen(PUERTO, () => {
    console.log(`El servidor WebSocket está escuchando en el puerto ${PUERTO}`);
});
