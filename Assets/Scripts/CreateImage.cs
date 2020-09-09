using UnityEngine;

public class CreateImage : MonoBehaviour {

    public GameObject imageObject;

    void OnEnable() {
        ReceiveTCP.messageRecieved += OnImageReceived;
        ReceiveTCP.messageStarted += OnNewMessageIncoming;
    }

    void OnDisable() {
        ReceiveTCP.messageRecieved -= OnImageReceived;
        ReceiveTCP.messageStarted -= OnNewMessageIncoming;
    }

    public void OnNewMessageIncoming() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public void OnImageReceived(ImageMessage currImage) {
        GameObject image = Instantiate(imageObject,transform);
        float zPos = 3 * transform.childCount;
        image.transform.localPosition = new Vector3(0, 0, zPos);
        Texture2D tex = new Texture2D(currImage.width, currImage.height, TextureFormat.ARGB32, false);
        tex.LoadRawTextureData(currImage.pixels);
        tex.Apply();
        Renderer rend = image.GetComponent<Renderer>();
        rend.material.mainTexture = tex;

        Debug.Log(currImage.pixels.Length);
    }
}
