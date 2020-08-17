
//from generator-core folder
//node app -f test/plugins

//close port
//sudo lsof -i :2222
//sudo kill -9 PID

//connection documentation https://photoshop-connection.readthedocs.io/en/latest/modules/photoshop.html

(function () {

	"use strict";

	var generator;
	var recieveMessage;

	function init(gen) {
		generator = gen;
		generator.addMenuItem("connection", "Connect", true, false);
		generator.addMenuItem("message", "Send Message", true, false);
		generator.onPhotoshopEvent("generatorMenuChanged", OnMenuClicked);
		generator.onPhotoshopEvent("closedDocument", OnDocumentClosed);
	}

	function SendTempMessage(){
		StartChildProcess("SendMessage.js");
	}

	function MoveImage(amount){
		var str = 
			"activeDocument.suspendHistory('', ''), sTT = stringIDToTypeID; \
			(ref = new ActionReference()).putProperty(sTT('historyState'), sTT('currentHistoryState')); \
			(dsc = new ActionDescriptor()).putReference(sTT('null'), ref), executeAction(sTT('delete'), dsc); \
			var doc = app.activeDocument; \
			for (var m = 0; m < doc.layers.length; m++){ \
			var theLayer = doc.layers[m]; \
			doc.activeLayer = theLayer; \
			theLayer.translate(0," + parseFloat(amount) + "); \
			}";
		generator.evaluateJSXString(str);
	}

	function StartChildProcess(script){
		//start child process to recieve messages TODO: CLOSE CONNECTION IDIOT
		var cp = require('child_process');
		const path = require('path');
		const dirPath = path.join(__dirname, script);
		return cp.fork(dirPath);
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
		StartRecieveMessageProcess();
	}

	function OnDocumentClosed(e){
		if (recieveMessage != null){
			recieveMessage.kill();
			recieveMessage = null;
			console.log("killed server socket");
		}
	}

	function OnMenuClicked(e){
		if (e.generatorMenuChanged.name == "message"){
			SendTempMessage();
		} else if (e.generatorMenuChanged.name == "connection"){
			OnConnectButtonPressed();
		}
	}

	exports.init = init;
}());