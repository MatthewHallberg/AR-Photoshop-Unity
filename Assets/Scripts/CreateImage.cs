using UnityEngine;

public class CreateImage : MonoBehaviour {

    public Renderer rend;

    void OnEnable() {
        ReceiveTCP.messageRecieved += OnImageReceived;
    }

    void OnDisable() {
        ReceiveTCP.messageRecieved -= OnImageReceived;
    }

    public void OnImageReceived(ImageMessage currImage) {
        print("loading texture");
        print("width: " + currImage.width);
        print("height: " + currImage.height);
        print("length: " + currImage.pixels.Length);
        Texture2D tex = new Texture2D(currImage.width, currImage.height, TextureFormat.ARGB32, false);
        tex.LoadRawTextureData(currImage.pixels);
        tex.Apply();
        rend.material.mainTexture = tex;
    }
}
