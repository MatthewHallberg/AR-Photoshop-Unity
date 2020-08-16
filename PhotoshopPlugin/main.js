
//from generator-core folder
//node app -f test/plugins

//close port
//sudo lsof -i :2222
//sudo kill -9 PID
(function () {

	"use strict";

	var generator;

	function init(gen) {
		generator = gen;
		generator.addMenuItem("color", "Change Color", true, false);
		generator.addMenuItem("connection", "Open Connection", true, false);
		generator.onPhotoshopEvent("generatorMenuChanged", menuClicked);
	}

	function menuClicked(e){
		if (e.generatorMenuChanged.name == "color"){
			console.log("MENU CLICKED");
			var str = "var color = app.foregroundColor; \
						color.rgb.red = " + Math.floor(Math.random() * 255) + "; \
						color.rgb.green = " + Math.floor(Math.random() * 255) + "; \
						color.rgb.blue = " + Math.floor(Math.random() * 255) + "; \
						app.foregroundColor = color;";
			generator.evaluateJSXString(str);
		} else if (e.generatorMenuChanged.name == "connection"){
			//start child process to recieve messages TODO: CLOSE CONNECTION IDIOT
			var cp = require('child_process');
			const path = require('path');
			const dirPath = path.join(__dirname, '/RecieveMessage.js');
			console.log(dirPath);
			var n = cp.fork(dirPath);
			//listen for messages from child process
			n.on('message', function(m) {
				var str = 
				"activeDocument.suspendHistory('', ''), sTT = stringIDToTypeID; \
				(ref = new ActionReference()).putProperty(sTT('historyState'), sTT('currentHistoryState')); \
				(dsc = new ActionDescriptor()).putReference(sTT('null'), ref), executeAction(sTT('delete'), dsc); \
				var doc = app.activeDocument; \
				for (var m = 0; m < doc.layers.length; m++){ \
				var theLayer = doc.layers[m]; \
				doc.activeLayer = theLayer; \
				theLayer.translate(0," + parseFloat(m) + "); \
				}";
			  console.log(parseFloat(m));
			  generator.evaluateJSXString(str);
			});
		}
	}

	exports.init = init;
}());