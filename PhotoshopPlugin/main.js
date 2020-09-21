
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

        var options = { clipToDocumentBounds: true};
		generator.getPixmap(currDocument.id, item.id, options)
		.then(function(pixmap) {

        	var startString = "@start@" + pixmap.width.toString() + "@" + pixmap.height.toString() + "@";
        	var endString = "@done@";

        	//if this is the first image add new message to beginning
        	if (layerIndex === 0){
				startString = "@new@" + startString;
        	} 

        	//if this is the last image and done message to end
        	if (layerIndex === currDocument.layers.length - 1){
				endString = endString +  "end@";
        	}

        	var startPixels = new Buffer(startString);
        	var pixels = pixmap.pixels;
        	var endPixels = new Buffer(endString);

        	SendMessageTCP(Buffer.concat([startPixels, pixels, endPixels]));
        	console.log("Sent: " + pixmap.pixels.length);

        	var nextIndex = layerIndex + 1;
        	if (nextIndex < currDocument.layers.length){
				SendLayerBuffer(nextIndex);
        	}
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
		var str = SelectAnyLayer() + 
			" \ var doc = app.activeDocument; \
			for (var m = 0; m < doc.layers.length; m++){ \
			var theLayer = doc.layers[m]; \
			doc.activeLayer = theLayer; \
			theLayer.translate(0," + parseFloat(amount) + "); \
			}"
		generator.evaluateJSXString(str);
	}

	//BUGFIX: case no layer is selected
	function SelectAnyLayer(){
		return "activeDocument.suspendHistory('', ''), sTT = stringIDToTypeID; \
			(ref = new ActionReference()).putProperty(sTT('historyState'), sTT('currentHistoryState')); \
			(dsc = new ActionDescriptor()).putReference(sTT('null'), ref), executeAction(sTT('delete'), dsc);";
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
		SelectAnyLayer();
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