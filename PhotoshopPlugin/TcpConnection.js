var net = require('net');
var port = 2223;
var host = '';
var conn = null;

//listen for messages from parent process
process.on('message',function(msg,info){
	if (conn == null){
		host = msg;
		CreateConnection();
	} else {
		conn.write(new Buffer(msg));
	}
});

function CreateConnection(){
	conn = net.createConnection(port,host);
	console.log("connection created at: " + host);
	conn.on('connect', function() {
		process.send('connected to server');
	});

	conn.on('data' , function (){
		console.log("Data received from the server: " , data);
	});

	conn.on('error', function(err) {
		console.log('Error in connection:', err);
	});

	conn.on('close', function() {
     	console.log('connection got closed, will try to reconnect');
    	conn.end();
	});

	conn.on('end' , function(){
    	console.log('Requested an end to the TCP connection');
	});
}

