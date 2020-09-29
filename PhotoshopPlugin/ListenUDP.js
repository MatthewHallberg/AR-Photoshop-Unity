var udp = require('dgram');

var PORT = 2222;

var server = udp.createSocket('udp4');
server.bind(PORT);

server.on('error',function(error){
  console.log('Error: ' + error);
  server.close();
});

//listen for UDP message
server.on('message',function(msg){
	process.send(msg.toString());
});

server.on('listening',function(){
  console.log('server listening at port:' + PORT);
});
