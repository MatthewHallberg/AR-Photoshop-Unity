var net = require('net');
var port = 2223;
var host = 'localhost';
var conn = net.createConnection(port ,host);

conn.on('connect', function() {
      console.log('connected to server');
});

conn.on('data' , function (){
      console.log("Data received from the server: " , data);
});

conn.on('error', function(err) {
      console.log('Error in connection:', err);
});

//listen for messages from parent process
process.on('message',function(msg,info){
  conn.write(new Buffer(msg));
});

conn.on('close', function() {
       console.log('connection got closed, will try to reconnect');
       conn.end();
});

conn.on('end' , function(){
      console.log('Requested an end to the TCP connection');
});