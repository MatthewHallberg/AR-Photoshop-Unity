using UnityEngine;

public class WorldImageManager : Singleton<WorldImageManager> {

    public GameObject imagePrefab;
    public GameObject holderPrefab;

    public void Create() {
        Transform handle = MoveHandle.Instance.transform;
        GameObject worldSpaceImages = Instantiate(holderPrefab, handle.position,handle.rotation);
        WorldImageBehavior worldImage = worldSpaceImages.GetComponent<WorldImageBehavior>();
        foreach (Transform img in CreateImage.Instance.transform) {
            GameObject image = Instantiate(img.gameObject, worldImage.imageParent);
        }
    }
}
