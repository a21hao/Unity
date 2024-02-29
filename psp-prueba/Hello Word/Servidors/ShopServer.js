const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 3003 });

let cursorIndex = 0; // Estado inicial del cursor

function sendCursorIndexToClients() {
  const data = JSON.stringify({ cursorIndex: cursorIndex });
  wss.clients.forEach(function each(client) {
    if (client.readyState === WebSocket.OPEN) {
      client.send(data);
    }
  });
}

wss.on('connection', function connection(ws) {
  console.log('Client connected');

  sendCursorIndexToClients();

  ws.on('message', function incoming(message) {
    console.log('Received message from a client: %s', message);

    try {
      const parsedMessage = JSON.parse(message);
      if (parsedMessage.hasOwnProperty('cursorIndex')) {
        cursorIndex = parsedMessage.cursorIndex;
        sendCursorIndexToClients();
      }
    } catch (error) {
      console.error('Error parsing message:', error);
    }
  });

  ws.on('close', function close() {
    console.log('Client disconnected');
  });
});

console.log('WebSocket server running on port 3003');
