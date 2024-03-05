const { spawn } = require('child_process');
const http = require('http');
const express = require('express');
const WebSocket = require('ws');

const app = express();
const server = http.createServer(app);
let wss;

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
    console.log(`Main server listening on port ${PORT}`);
    
    startChildServer('Servidors/ChatServer.js');
    startChildServer('Servidors/GameServer.js');
    startChildServer('Servidors/ShopServer.js');

    // Iniciar el servidor WebSocket una vez que los servidores hijos estén listos
    startWebSocketServer();
});

let chatServer;
let gameServer;
let shopServer;

function startChildServer(serverScript) {
    const serverInstance = spawn('node', [serverScript]);

    serverInstance.stdout.on('data', (data) => {
        console.log(`Child process stdout (${serverScript}): ${data}`);
    });

    serverInstance.stderr.on('data', (data) => {
        console.error(`Child process stderr (${serverScript}): ${data}`);
    });

    serverInstance.on('close', (code) => {
        console.log(`Child process (${serverScript}) exited with code ${code}`);
        // Reiniciar el servidor WebSocket cuando se detiene un servidor hijo
        restartWebSocketServer();
    });

    switch (serverScript) {
        case 'Servidors/ChatServer.js':
            chatServer = serverInstance;
            console.log(`Chat server connected`);
            break;
        case 'Servidors/GameServer.js':
            gameServer = serverInstance;
            console.log(`Game server connected`);
            break;
        case 'Servidors/ShopServer.js':
            shopServer = serverInstance;
            console.log(`Shop server connected`);
            break;
        default:
            console.error(`Unrecognized server script: ${serverScript}`);
            break;
    }
}

function startWebSocketServer() {
    wss = new WebSocket.Server({ server });

    wss.on('connection', (ws) => {
        console.log('Client connected');
      
        ws.on('message', (message) => {
            try {
                const { serverName, signal } = JSON.parse(message);
                console.log(`Received message from client: ${serverName} ${signal}`);

                // Actuar según la señal recibida
                switch (signal) {
                    case 'stop':
                        stopServer(serverName);
                        break;
                    case 'start':
                        startServer(serverName);
                        break;
                    default:
                        console.error(`Unrecognized signal: ${signal}`);
                        break;
                }
            } catch (error) {
                console.error('Error parsing JSON message:', error);
            }
        });

        ws.on('close', () => {
            console.log('Client disconnected');
        });
    });
}

function restartWebSocketServer() {
    if (wss) {
        wss.close(() => {
            console.log('WebSocket server closed');
            startWebSocketServer();
        });
    }
}

function stopServer(serverName) {
    let childServer;
    switch (serverName) {
        case 'chat':
            childServer = chatServer;
            break;
        case 'game':
            childServer = gameServer;
            break;
        case 'shop':
            childServer = shopServer;
            break;
        default:
            console.error(`Unrecognized server name: ${serverName}`);
            return;
    }

    if (childServer) {
        console.log(`Stopping ${serverName} server...`);
        childServer.kill('SIGINT');
    } else {
        console.error(`Server ${serverName} not found`);
    }
}

function startServer(serverName) {
    let childServer;
    switch (serverName) {
        case 'chat':
            childServer = chatServer;
            break;
        case 'game':
            childServer = gameServer;
            break;
        case 'shop':
            childServer = shopServer;
            break;
        default:
            console.error(`Unrecognized server name: ${serverName}`);
            return;
    }

    if (!childServer) {
        console.log(`Starting ${serverName} server...`);
        startChildServer(`Servidors/${serverName.charAt(0).toUpperCase() + serverName.slice(1)}Server.js`);
    } else {
        console.error(`Server ${serverName} is already running`);
    }
}
