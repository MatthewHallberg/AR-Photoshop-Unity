var udp = require('dgram');

var server = udp.createSocket('udp4');

server.on('error',function(error){
  console.log('Error: ' + error);
  server.close();
});

server.on('message',function(msg,info){
  console.log('Data received from client : ' + msg.toString());
  process.send(msg.toString());
});

server.on('listening',function(){
  var address = server.address();
  var port = address.port;
  var family = address.family;
  var ipaddr = address.address;
  console.log('Server is listening at port' + port);
  console.log('Server ip :' + ipaddr);
  console.log('Server is IP4/IP6 : ' + family);
});

server.on('close',function(){
  console.log('Socket closed');
});

server.bind(2222);
