using UnityEngine;
using Vuforia;

public class TargetController : Singleton<TargetController> {

    public readonly float ImageTargetSizeMeters = 0.2f;

    public bool imageTransferError { get; set; }

    public Camera targetCamera;
    public Transform imageParent;

    void Start() {
        //turn off image target camera until we need it
        targetCamera.gameObject.SetActive(false);
    }

    void OnEnable() {
        ReceiveTCP.messageComplete += CreateImageTarget;
    }

    void OnDisable() {
        ReceiveTCP.messageComplete -= CreateImageTarget;
    }

    void CreateImageTarget() {

        //if an image doesnt come through dont create tracker
        if (imageTransferError) {
            return;
        }

        //set physical size of parent to match physical image size
        imageParent.localScale = Vector3.one * ImageTargetSizeMeters;

        //get image target texture from second camera
        targetCamera.gameObject.SetActive(true);
        targetCamera.Render();
        Texture2D imageTargetTexture = RenderTexutreToTexture2D(targetCamera.activeTexture);
        targetCamera.gameObject.SetActive(false);

        //create runtime tracker
        var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        var runtimeImageSource = objectTracker.RuntimeImageSource;
        bool success = runtimeImageSource.SetImage(imageTargetTexture, ImageTargetSizeMeters, "tracker");
        Debug.Log("Tracker Created: " + success);
        var dataset = objectTracker.CreateDataSet();
        dataset.CreateTrackable(runtimeImageSource, gameObject);
        objectTracker.ActivateDataSet(dataset);

        if (success) {
            OnTargetCreated();
        }
    }

    void OnTargetCreated() {
        //vuforia will active renderers on detection
        SetRenderersActive(false);
        //make image slighty bigger to cover entire photoshop image
        imageParent.localScale += Vector3.one * .01f;
    }

    Texture2D RenderTexutreToTexture2D(RenderTexture rTex) {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    void SetRenderersActive(bool active) {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
            rend.enabled = active;
        }
    }
}
