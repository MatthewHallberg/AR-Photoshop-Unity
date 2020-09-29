using UnityEngine;

public class MovePhotoshop : MonoBehaviour {

    const float MOVE_SPEED = 4f;
    const float UNITY_DOCUMENT_HANDLE_HEIGHT = .2f;

    Camera mainCam;
    Vector3 startPosition;
    bool shouldMove = true;

    void Start() {
        startPosition = transform.localPosition;
        mainCam = Camera.main;
    }

    void Update() {
        if (shouldMove) {
           transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * MOVE_SPEED);
            if (Vector3.Distance(transform.localPosition, startPosition) < .01) {
                transform.localPosition = startPosition;
            }
        }
    }

   void OnMouseDown() {
        shouldMove = false;
    }

    void OnMouseUp() {
        shouldMove = true;
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
}
