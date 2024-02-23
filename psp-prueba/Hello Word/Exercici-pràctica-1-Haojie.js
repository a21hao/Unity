const http = require('http');
const WebSocket = require('ws');

const PUERTO = 3000;

const server = http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('Servidor WebSocket\n');
});

const wss = new WebSocket.Server({ server });

wss.on('connection', (ws) => {
    console.log('Cliente conectado');
    ws.send("Hello World");

    ws.on('message', (mensaje) => {
        console.log(`Mensaje recibido: ${mensaje}`);
    });

    ws.on('close', () => {
        console.log('Cliente desconectado');
    });
});

server.listen(PUERTO, () => {
    console.log(`El servidor WebSocket está escuchando en el puerto ${PUERTO}`);
});
