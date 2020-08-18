
//from generator-core folder
//node app -f test/plugins

//close port
//sudo lsof -i :2222
//sudo kill -9 PID

//connection documentation https://photoshop-connection.readthedocs.io/en/latest/modules/photoshop.html

(function () {

	"use strict";

	var udp = require('dgram');
	var tcp = require('net');

	var generator;
	var recieveMessage;
	var tcpConnection;

	function init(gen) {
		generator = gen;
		generator.addMenuItem("connection", "Connect", true, false);
		generator.onPhotoshopEvent("generatorMenuChanged", OnMenuClicked);
		generator.onPhotoshopEvent("closedDocument", OnDocumentClosed);
	}

	function ExportLayers(){
		console.log("exporting layers...");

		var documentId;
		var layerId;

        generator.getDocumentInfo()
            .then(function (document) {
                documentId = document.id;
                layerId = document.layers[0].id;
            })
            .then(function() {
				return generator.getPixmap(documentId, layerId, { clipToDocumentBounds: true });
            })
            .then(function(pixmap) {
            	console.log("bits per channel: " + pixmap.bitsPerChannel);
            	console.log("width: " + pixmap.width);
            	console.log("height: " + pixmap.height);
            	SendMessageTCP(pixmap.pixels);
            	//SendMessage(Buffer.from("TESTING123", 'utf8'));
            });
	}

	function SendMessageTCP(pixels){
		console.log("sending pixels...");
		//socket for sending image via TCP
		if (tcpConnection != null){
			tcpConnection.send(pixels);
		}
	}	

	function SendMessageUDP(message){
		var client = udp.createSocket({type:"udp4", reuseAddr:true});
		client.bind(function() {
			client.setBroadcast(true);
			console.log("sending message...");
			var buffer = new Buffer(message);
			client.send(buffer, 0, buffer.length, 2223, "255.255.255.255");
		});
	}

	function MoveImage(amount){
		SelectAnyLayer();
		var str = 
			"var doc = app.activeDocument; \
			for (var m = 0; m < doc.layers.length; m++){ \
			var theLayer = doc.layers[m]; \
			doc.activeLayer = theLayer; \
			theLayer.translate(0," + parseFloat(amount) + "); \
			}";
		generator.evaluateJSXString(str);
	}

	//BUGFIX: case no layer is selected
	function SelectAnyLayer(){
		var str = "activeDocument.suspendHistory('', ''), sTT = stringIDToTypeID; \
			(ref = new ActionReference()).putProperty(sTT('historyState'), sTT('currentHistoryState')); \
			(dsc = new ActionDescriptor()).putReference(sTT('null'), ref), executeAction(sTT('delete'), dsc);";
		generator.evaluateJSXString(str);
	}

	function StartChildProcess(script){
		//start child process to listen for packets
		var cp = require('child_process');
		const path = require('path');
		const dirPath = path.join(__dirname, script);
		return cp.fork(dirPath);
	}

	function StartTCPConnection(){
		if (tcpConnection == null){
			tcpConnection = StartChildProcess("TcpConnection.js");
		}
	}

	function StartRecieveMessageProcess(){
		if (recieveMessage == null){
			recieveMessage = StartChildProcess("RecieveMessage.js");
			//listen for messages from child process
			recieveMessage.on('message', function(m) {
				MoveImage(m);
			});
		}
	}

	function OnConnectButtonPressed(){
		StartTCPConnection();
		StartRecieveMessageProcess();
		ExportLayers();
	}

	function OnDocumentClosed(e){
		if (recieveMessage != null){
			recieveMessage.kill();
			recieveMessage = null;
			console.log("killed server socket");
		}
	}

	function OnMenuClicked(e){
		if (e.generatorMenuChanged.name == "connection"){
			OnConnectButtonPressed();
		}
	}

	exports.init = init;
}());