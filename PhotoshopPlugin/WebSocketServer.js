const crypto = require('crypto');
const express = require('express');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();

const server = createServer(app);
const wss = new WebSocket.Server({ server });

wss.on('connection', function(ws) {
  
  process.send('connected');

  // send "hello world" interval
  const textInterval = setInterval(() => ws.send("hello world!"), 1000);

  ws.on('message', function(data) {
    if (typeof(data) === "string") {
      process.send(data);
    } else {
      console.log("client sent non string data??");
    }
  });

  ws.on('close', function() {
    process.send('disconnected');
    clearInterval(textInterval);
  });
});

server.listen(2223, function() {
  console.log('Listening on http://localhost:2223');
});