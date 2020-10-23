using UnityEngine;

public class HandleAnchor : MonoBehaviour {

    Transform imageHandle;

    void Start() {
        imageHandle = ImageHandle.Instance.GetTransform();
    }

    void Update() {
        imageHandle.position = transform.position;
        imageHandle.rotation = transform.rotation;
    }
}
