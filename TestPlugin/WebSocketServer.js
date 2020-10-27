const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 2223 });

var ConnectedSocket;

wss.on('connection', function(ws) {

  process.send('connected');

  ConnectedSocket = ws;

  ws.on('message', function(data) {
    if (typeof(data) === "string") {
      process.send(data);
    } else {
      console.log("client sent non string data??");
    }
  });

  ws.on('close', function() {
    process.send('disconnected');
  });
});

process.on('message',function(msg,info){
    ConnectedSocket.send(msg);
});