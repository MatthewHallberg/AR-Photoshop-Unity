var udp = require('dgram');

var PORT = 2221;

//listen for message from parent
process.on('message',function(msg,info){
	var client = udp.createSocket({type:"udp4", reuseAddr:true});
	client.bind(function() {
		client.setBroadcast(true);
		console.log("Sending IP address via UDP: " + msg);
		var buffer = new Buffer(msg);
		client.send(buffer, 0, buffer.length, PORT, "255.255.255.255");
	});
});