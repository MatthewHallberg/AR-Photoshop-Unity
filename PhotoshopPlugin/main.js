
//from generator-core folder
//node app -f test/plugins

//close port
//sudo lsof -i :2222
//sudo kill -9 PID

//connection documentation https://photoshop-connection.readthedocs.io/en/latest/modules/photoshop.html

(function () {

	"use strict";

	var generator;
	var listenUDP;
	var tcpConnection;

	var tcpConnectionMade = false;

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
            	console.log("width: " + pixmap.width);
            	console.log("height: " + pixmap.height);
            	console.log("length: " + pixmap.pixels.length);

            	SendMessageTCP("start," + pixmap.width + "," + pixmap.height + ",");
            	SendMessageTCP(pixmap.pixels);
            	SendMessageTCP(",done,");
            });
	}

	function SendMessageTCP(pixels){
		//socket for sending image via TCP
		if (tcpConnection != null){
			tcpConnection.send(pixels);
		}
	}	

	function SendMessageUDP(message){
		var sendUDP = StartChildProcess("SendUDP.js");
		sendUDP.send(message);
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

	function OnTCPMessage(msg){
		console.log(msg);
		if (msg == "connected"){
			tcpConnectionMade = true;
			StartListenForMove();
			ExportLayers();
		} else if (msg == "disconnected"){
			tcpConnectionMade = false;
			tcpConnection = null;
		}
	}

	function StartTCPConnection(){
		if (tcpConnection == null){
			tcpConnection = StartChildProcess("TcpConnection.js");
			tcpConnection.on('message', function(m) {
				OnTCPMessage(m);
			});
		}
	}

	function CloseTCPConnection(){
		if (tcpConnection == null){
			tcpConnection.kill();
			tcpConnection = null;
		}
	}

	function StartListenForMove(){
		StopListenUDP();
		listenUDP = StartChildProcess("listenUDP.js");
		//listen for messages from child process
		listenUDP.on('message', function(m) {
			MoveImage(m);
		});
	}

	function StartListemForUnityIP(){
		StopListenUDP();
		listenUDP = StartChildProcess("listenUDP.js");
		//listen for messages from child process
		listenUDP.on('message', function(m) {
			OnUnityIPAddressReceieved(m);
		});
	}

	function StopListenUDP(){
		if (listenUDP != null){
			listenUDP.kill();
			listenUDP = null;
			console.log("stopped udp listener");
		}
	}

	function OnUnityIPAddressReceieved(IPAddress){
		console.log("Got Unity IP address: " + IPAddress);
		StartTCPConnection();
		tcpConnection.send(IPAddress);
	}

	function OnPhotoshopIPAddressRecieved(IPAddress){
		console.log("Got local IP address: " + IPAddress);
		StartListemForUnityIP();
		SendMessageUDP(IPAddress);
	}

	function GetPhotoshopIPAddress(){
		var GetIPAddressPhotoshop = StartChildProcess("IPAddress.js");
		GetIPAddressPhotoshop.on('message', function(ip) {
			OnPhotoshopIPAddressRecieved(ip);
		});
	}

	function OnConnectButtonPressed(){
		if (tcpConnectionMade){
			ExportLayers();
		} else {
			GetPhotoshopIPAddress();
		}
	}

	function OnDocumentClosed(e){
		StopListenUDP();
		CloseTCPConnection();
	}

	function OnMenuClicked(e){
		if (e.generatorMenuChanged.name == "connection"){
			OnConnectButtonPressed();
		}
	}

	exports.init = init;
}());