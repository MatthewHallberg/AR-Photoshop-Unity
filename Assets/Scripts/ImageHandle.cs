using UnityEngine;

public class ImageHandle : Singleton<ImageHandle> {

    public Transform GetTransform() {
        return transform;
    }
}
