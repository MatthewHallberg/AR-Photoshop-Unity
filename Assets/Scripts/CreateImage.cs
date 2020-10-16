using UnityEngine;

public class CreateImage : MonoBehaviour {

    const float DOCUMENT_SIZE = 1024f;
    const float Z_DISTANCE = .0001f;

    public GameObject imageObject;

    public Renderer testRend;

    public void OnNewMessageIncoming() {
        //clear error state
        TargetController.Instance.imageTransferError = false;
        TargetController.Instance.imageTrackerCreated = false;
        MoveHandle.Instance.EnableVisuals(true);
        //delete old images
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public void OnImageReceived(ImageMessage currImage) {

        if (TargetController.Instance.imageTransferError) {
            return;
        }

        try {
            //create image plane
            GameObject image = Instantiate(imageObject, transform);
            //position object behind the last one
            float zPos = Z_DISTANCE * transform.childCount;
            image.transform.localPosition = new Vector3(0, 0, zPos);
            //scale object based on size of photoshop layer
            image.transform.localScale = new Vector3((float)currImage.width / DOCUMENT_SIZE, (float)currImage.height / DOCUMENT_SIZE, 1);

            //create texture from pixels
            Texture2D tex = new Texture2D(currImage.width, currImage.height, TextureFormat.ARGB32, false);

            //var colorArray = new Color32[currImage.pixels.Length / 4];
            //for (var i = 0; i < currImage.pixels.Length; i += 4) {
            //    var color = new Color32(currImage.pixels[i + 1], currImage.pixels[i + 2], currImage.pixels[i + 3], currImage.pixels[i + 0]);
            //    colorArray[i / 4] = color;
            //}
            //tex.SetPixels32(colorArray);


            tex.LoadRawTextureData(currImage.pixels);

            //tex.SetPixels32()
            //tex.LoadImage(currImage.pixels);
            tex.Apply();
            //apply texture to material instance
            Renderer rend = image.GetComponent<Renderer>();
            rend.material.mainTexture = tex;

            //test
            testRend.material.mainTexture = tex;

            //set layer to image target for getting tracker image
            image.layer = 8;

            Debug.Log(currImage.pixels.Length);

        } catch (UnityException ex) {
            Debug.Log("Image send error: " + ex);
            Debug.Log("If layer is text try converting it to a shape???");
            TargetController.Instance.imageTransferError = true;
            ConnectionManager.Instance.SendMessageToPhotoshop("Image send error: please modify the document and try again.");
        }
    }
}
