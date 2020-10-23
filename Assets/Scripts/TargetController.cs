using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class TargetController : Singleton<TargetController> {

    public readonly float ImageTargetSizeMeters = 0.2f;

    public bool imageTransferError { get; set; }
    public bool imageTrackerCreated { get; set; }

    public Camera targetCamera;

    void OnEnable() {
        WebSocketConnection.messageComplete += CreateImageTarget;
        WebSocketConnection.messageStarted += OnNewMessageIncoming;
    }

    void OnDisable() {
        WebSocketConnection.messageComplete -= CreateImageTarget;
        WebSocketConnection.messageStarted -= OnNewMessageIncoming;
    }

    void Start() {
        //turn off image target camera until we need it
        targetCamera.gameObject.SetActive(false);
    }

    void OnNewMessageIncoming(int unused) {
        var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        objectTracker.Stop();
        objectTracker.DestroyAllDataSets(false);
    }

    void CreateImageTarget() {
        StartCoroutine(CreateTrackerRoutine());
    }

    IEnumerator CreateTrackerRoutine() {

        yield return new WaitForEndOfFrame();

        //if an image doesnt come through dont create tracker
        if (imageTransferError) {
            yield break;
        }

        //get image target texture from second camera
        MoveHandle.Instance.EnableImageVisuals(true);
        targetCamera.transform.position = MoveHandle.Instance.ImageParent.position;
        yield return new WaitForEndOfFrame();
        targetCamera.Render();
        Texture2D imageTargetTexture = RenderTexutreToTexture2D(targetCamera.activeTexture);
        targetCamera.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        //TEST:
#if UNITY_EDITOR
        DebugWriteImageToFIle(imageTargetTexture);
#endif

        //create runtime tracker
        var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        var runtimeImageSource = objectTracker.RuntimeImageSource;
        bool success = runtimeImageSource.SetImage(imageTargetTexture, ImageTargetSizeMeters, "tracker");
        Debug.Log("Tracker Created: " + success);
        var dataset = objectTracker.CreateDataSet();
        dataset.CreateTrackable(runtimeImageSource, gameObject);
        objectTracker.ActivateDataSet(dataset);
        objectTracker.Start();

        if (success) {
            Debug.Log("Image target created successful...");

            if (!IsTestScene()) {
                OnTargetCreated();
            }

            Message.Instance.ShowMessage("Tracker created!");
            yield return new WaitForSeconds(1f);
            Message.Instance.CloseMessage();
        }
    }

    void OnTargetCreated() {
        imageTrackerCreated = true;
        //vuforia will active renderers on detection
        SetRenderersActive(false);
    }

    bool IsTestScene() {
        return SceneManager.GetActiveScene().name == "test";
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
