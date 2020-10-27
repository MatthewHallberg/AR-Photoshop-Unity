//from generator-core folder
//node app -f test/plugins

//close port
//sudo lsof -i :2222
//sudo kill -9 PID

//connection documentation https://photoshop-connection.readthedocs.io/en/latest/modules/photoshop.html

(function () {

	"use strict";

	var generator;
	var webSocketServer;
	var webSocketConnectionMade = false;
	var currDocument;

	function init(gen) {
		generator = gen;
		generator.addMenuItem("connection", "Connect", true, false);
		generator.onPhotoshopEvent("generatorMenuChanged", OnMenuClicked);
		generator.onPhotoshopEvent("closedDocument", OnDocumentClosed);
	}

	function ExportLayers(){
		console.log("exporting layers...");
        generator.getDocumentInfo()
        .then(function (document) {
        	currDocument = document;
        	SendLayerBuffer(0);
        });
	}

	function SendLayerBuffer(layerIndex){

		var item = currDocument.layers[layerIndex];

        var options = {clipToDocumentBounds: true};
		generator.getPixmap(currDocument.id, item.id, options)
		.then(function(pixmap) {

        	//if this is the first image send number of layers
        	if (layerIndex === 0){
				webSocketServer.send(currDocument.layers.length.toString());
        	}

        	console.log("Sent: " + pixmap.pixels.length);

        	var imageLayer = {
  				width: pixmap.width,
				height: pixmap.height,
				top: pixmap.bounds.top,
				bottom: pixmap.bounds.bottom,
				left: pixmap.bounds.left,
				right: pixmap.bounds.right,
				pixels: pixmap.pixels						
			};

			webSocketServer.send(JSON.stringify(imageLayer));

        	var nextIndex = layerIndex + 1;
        	if (nextIndex < currDocument.layers.length){
        		SendLayerBuffer(nextIndex);
        	}
		});
	}

	function SendMessageUDP(message){
		var sendUDP = StartChildProcess("SendUDP.js");
		sendUDP.send(message);
	}

	function MessageFromUnity(message){
		var jsxStr = "alert('" + message + "');";
		generator.evaluateJSXString(jsxStr);
	}

	function StartChildProcess(script){
		//start child process to listen for packets
		var cp = require('child_process');
		const path = require('path');
		const dirPath = path.join(__dirname, script);
		return cp.fork(dirPath);
	}

	function OnSocketMessage(msg){
		console.log(msg);
		if (msg == "connected"){
			webSocketConnectionMade = true;
			ExportLayers();
		} else if (msg == "disconnected"){
			webSocketConnectionMade = false;
			CloseSocketConnection();
		} else {
			MessageFromUnity(msg);
		}
	}

	function StartWebSocket(){
		if (webSocketServer == null){
			webSocketServer = StartChildProcess("WebSocketServer.js");
			webSocketServer.on('message', function(m) {
				OnSocketMessage(m);
			});
		}
	}

	function CloseSocketConnection(){
		if (webSocketServer != null){
			webSocketServer.kill();
			webSocketServer = null;
			webSocketConnectionMade = false;
		}
	}

	function GetPhotoshopIPAddress(){
		var GetIPAddressPhotoshop = StartChildProcess("IPAddress.js");
		GetIPAddressPhotoshop.on('message', function(ip) {
			console.log("Got local IP address: " + ip);
			SendMessageUDP(ip);
		});
	}

	function OnConnectButtonPressed(){
		if (webSocketConnectionMade){
			ExportLayers();
		} else {
			StartWebSocket();
			GetPhotoshopIPAddress();
		}
	}

	function OnDocumentClosed(e){
		CloseSocketConnection();
	}

	function OnMenuClicked(e){
		if (e.generatorMenuChanged.name == "connection"){
			OnConnectButtonPressed();
		}
	}

	exports.init = init;
}());