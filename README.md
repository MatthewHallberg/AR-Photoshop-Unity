# AR-Photoshop-Unity

![](readme.png?raw=true "Title")

<b>Instuctions:</b>
<p>Take the "TestPlugin" folder and place it in Applications->AdobePhotoshop2021->Plug-ins->Generator

Open Photoshop and go to Photoshop->Preferences->Plug-ins
<p>-Check "Enable Generator"
<p>-Check "Enable Remote Connections"
<p>-Change the password to "password"

*Make sure the App and Photoshop are connected to the same WIFI network.*
<p>The plugin sends a broadcast message to the device with its IP for the web socket connection
so you may need to turn off cellular data to get the broadcast message???

Caveats:
<p>1.)This only works if the entire document has enough feature points to track so blank colors or designs without a lot of contrast will not be tracked by the app.
<p>2.)Currently this is only set up to work with Photoshop docs that are 512x512 pixels.
