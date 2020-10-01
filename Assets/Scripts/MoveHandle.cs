using UnityEngine;

public class MoveHandle : MonoBehaviour {

    const float HANDLE_SPEED = 4f;
    const float IMAGE_SPEED = 10f;
    const float UNITY_DOCUMENT_HANDLE_HEIGHT = .2f;

    public Transform ImageParent;
    public Transform TransparentImages;

    Vector3 imageTopPos;
    Vector3 imageBottomPos;
    Vector3 desiredImagePos;

    Camera mainCam;
    Vector3 startPosition;
    bool shouldMoveHandle = true;

    void Start() {
        startPosition = transform.localPosition;
        mainCam = Camera.main;
        SetImagePositions();
    }

    void Update() {
        if (shouldMoveHandle) {
           transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * HANDLE_SPEED);
            if (Vector3.Distance(transform.localPosition, startPosition) < .01) {
                transform.localPosition = startPosition;
            }
        }

        ImageParent.localPosition = Vector3.Lerp(ImageParent.localPosition, desiredImagePos, Time.deltaTime * IMAGE_SPEED);
    }

   void OnMouseDown() {
        shouldMoveHandle = false;
        desiredImagePos = imageTopPos;
    }

    void OnMouseUp() {
        shouldMoveHandle = true;
        desiredImagePos = imageBottomPos;
    }

    void OnMouseDrag() {

        //get world position of screen touch
        Transform imageTarget = transform.parent;
        float distance = Vector3.Distance(imageTarget.position,mainCam.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 worldTouchPos = mainCam.ScreenToWorldPoint(mousePos);

        //convert to localPosition
        Vector3 localPos = transform.parent.InverseTransformPoint(worldTouchPos);

        //only allow handle to move on local z axis (up and down)
        Vector3 currPos = startPosition;
        currPos.z = localPos.z;

        if (localPos.z >= startPosition.z && localPos.z <= startPosition.z + UNITY_DOCUMENT_HANDLE_HEIGHT) {
            transform.localPosition = currPos;
        }
    }

    void SetImagePositions() {
        imageTopPos = ImageParent.localPosition;
        ImageParent.position = TransparentImages.position;
        imageBottomPos = ImageParent.localPosition;
        desiredImagePos = imageBottomPos;
    }
}
