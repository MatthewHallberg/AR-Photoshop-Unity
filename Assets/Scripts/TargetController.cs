using System.IO;
using UnityEngine;
using Vuforia;

public class TargetController : Singleton<TargetController> {

    public readonly float ImageTargetSizeMeters = 0.2f;

    public bool imageTransferError { get; set; }
    public bool imageTrackerCreated { get; set; }

    public Camera targetCamera;

    void Start() {
        //turn off image target camera until we need it
        targetCamera.gameObject.SetActive(false);
    }

    void CreateImageTarget() {

        //if an image doesnt come through dont create tracker
        if (imageTransferError) {
            return;
        }

        //get image target texture from second camera
        MoveHandle.Instance.EnableVisuals(true);
        targetCamera.Render();
        Texture2D imageTargetTexture = RenderTexutreToTexture2D(targetCamera.activeTexture);
        targetCamera.gameObject.SetActive(false);

        //TEST:
        //DebugWriteImageToFIle(imageTargetTexture);

        //create runtime tracker
        var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        var runtimeImageSource = objectTracker.RuntimeImageSource;
        bool success = runtimeImageSource.SetImage(imageTargetTexture, ImageTargetSizeMeters, "tracker");
        Debug.Log("Tracker Created: " + success);
        var dataset = objectTracker.CreateDataSet();
        dataset.CreateTrackable(runtimeImageSource, gameObject);
        objectTracker.ActivateDataSet(dataset);

        if (success) {
            Debug.Log("Image target created successful...");
            OnTargetCreated();
        }
    }

    void OnTargetCreated() {
        imageTrackerCreated = true;
        //vuforia will active renderers on detection
        SetRenderersActive(false);
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

    void DebugWriteImageToFIle(Texture2D trackerImage) {
        byte[] imageBytes = trackerImage.EncodeToPNG();
        File.WriteAllBytes(Application.streamingAssetsPath + "/test.png", imageBytes);
        Debug.Log("Image Target wrote to file!");
    }
}
