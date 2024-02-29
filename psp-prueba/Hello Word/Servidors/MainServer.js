const { spawn } = require('child_process');
const http = require('http');
const express = require('express');

const app = express();
const server = http.createServer(app);

// Start the main server
const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
    console.log(`Main server listening on port ${PORT}`);
    
    // Inicia el servidor de chat
    startChildServer('Servidors/ChatServer.js');
    
    // Inicia el servidor de juego
    startChildServer('Servidors/GameServer.js');

    // Inicia el servidor de tienda
    startChildServer('Servidors/ShopServer.js');
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
    });

    // Asigna la instancia de servidor al servidor correspondiente
    switch (serverScript) {
        case 'ChatServer.js':
            chatServer = serverInstance;
            break;
        case 'GameServer.js':
            gameServer = serverInstance;
            break;
        case 'ShopServer.js':
            shopServer = serverInstance;
            break;
        default:
            console.error(`Unrecognized server script: ${serverScript}`);
            break;
    }
}

// Maneja la terminación del proceso principal para cerrar los servidores secundarios
process.on('SIGINT', () => {
    console.log('Main process terminated. Closing child servers.');
    if (chatServer) {
        chatServer.kill('SIGINT');
    }
    if (gameServer) {
        gameServer.kill('SIGINT');
    }
    if (shopServer) {
        shopServer.kill('SIGINT');
    }
    process.exit();
});
