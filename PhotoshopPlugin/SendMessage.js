var udp = require('dgram');

var PORT = 2223;

var client = udp.createSocket({type:"udp4", reuseAddr:true});

client.bind(function() {
    client.setBroadcast(true);
    SendPacket("yoo");
});

function SendPacket(message){
	console.log("sending message: " + message);
	var buffer = new Buffer(message);
	client.send(buffer, 0, buffer.length, PORT, "255.255.255.255");
}

