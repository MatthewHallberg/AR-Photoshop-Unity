const crypto = require('crypto');
const express = require('express');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();

const server = createServer(app);
const wss = new WebSocket.Server({ server });

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

server.listen(2223, function() {
  console.log('Listening on http://localhost:2223');
});