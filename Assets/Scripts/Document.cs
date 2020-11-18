using UnityEngine;

public class Document : MonoBehaviour {

    public void SetDocumentSize(DocumentInfo docInfo) {

        if (docInfo == null) {
            return;
        }

        float width = docInfo.width;
        float height = docInfo.height;

        bool isLandscape = width > height;
        float aspectRatio = Mathf.Min(height, width) / Mathf.Max(height, width);

        if (isLandscape) {
            transform.localScale = new Vector3(1, aspectRatio, 1);
        } else {
            transform.localScale = new Vector3(aspectRatio, 1, 1);
        }
    }
}
