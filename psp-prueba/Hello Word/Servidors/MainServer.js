const { spawn } = require('child_process');
const http = require('http');
const express = require('express');

const app = express();
const server = http.createServer(app);

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
    console.log(`Main server listening on port ${PORT}`);
    
    startChildServer('Servidors/ChatServer.js');
    
    startChildServer('Servidors/GameServer.js');

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

app.post('/stop/:serverName', (req, res) => {
    const serverName = req.params.serverName;

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
            return res.status(400).send('Server not found');
    }

    if (childServer) {
        console.log(`Stopping ${serverName} server...`);
        childServer.kill('SIGINT');
        return res.send(`${serverName} server stopped.`);
    } else {
        return res.status(400).send('Server not found');
    }
});

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
