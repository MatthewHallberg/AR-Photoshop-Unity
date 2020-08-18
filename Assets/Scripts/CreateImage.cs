using UnityEngine;

public class CreateImage : MonoBehaviour {

    public Renderer rend;

    void OnEnable() {
        ReceiveTCP.messageRecieved += OnImageReceived;
    }

    void OnDisable() {
        ReceiveTCP.messageRecieved -= OnImageReceived;
    }

    public void OnImageReceived(byte[] pixels) {
        print("loading texture");
        //tried etc2, rgba32 (closest), 
        Texture2D tex = new Texture2D(762, 786, TextureFormat.ARGB32, false);
        tex.LoadRawTextureData(pixels);
        tex.Apply();
        rend.material.mainTexture = tex;
    }
}
