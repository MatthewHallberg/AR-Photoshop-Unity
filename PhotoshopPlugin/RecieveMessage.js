var udp = require('dgram');

var PORT = 2222;

var server = udp.createSocket({type:"udp4", reuseAddr:true});
server.bind(PORT);

server.on('error',function(error){
  console.log('Error: ' + error);
  server.close();
});

server.on('message',function(msg,info){
  console.log('Data Received: ' + msg.toString());
  process.send(msg.toString());
});

server.on('listening',function(){
  console.log('server listening at port:' + PORT);
});