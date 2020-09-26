using System.Collections;
using UnityEngine;

public class MovePhotoshop : MonoBehaviour {

    const float MOVE_SPEED = 4f;
    const float MAX_PHOTOSHOP_MOVE = 7.5f;
    const float UNITY_DOCUMENT_HANDLE_HEIGHT = .15f;

    readonly float DELAY = .4f;

    float currDocValue = 0;
    float currValToSend;

    float photoshopYPos;

    Camera mainCam;
    Vector3 startPosition;
    bool shouldMove = true;

    void Start() {
        startPosition = transform.localPosition;
        mainCam = Camera.main;
        StartCoroutine(SendMoveMessageRoutine());
    }

    void Update() {
        if (shouldMove) {
           transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * MOVE_SPEED);
        }

        CalcPhotoshopPosition();
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

    void CalcPhotoshopPosition() {
        //remap handle position to photoshop coordinates
        float currVal = transform.localPosition.z;
        float from1 = startPosition.z;
        float to1 = startPosition.z + UNITY_DOCUMENT_HANDLE_HEIGHT;
        float from2 = 0;
        float to2 = MAX_PHOTOSHOP_MOVE;

        photoshopYPos = ExtensionMethods.Remap(currVal, from1, to1, from2, to2);

        float valToSend = photoshopYPos - currDocValue;
        currValToSend += valToSend;
        currDocValue = photoshopYPos;
    }

    public void OnSliderValueChanged(float val) {
        float valToSend = val - currDocValue;
        currValToSend += valToSend;
        currDocValue = val;
    }

    IEnumerator SendMoveMessageRoutine() {
        while (true) {
            if (!currValToSend.Equals(0)) {
                ConnectionManager.Instance.SendUDPMessage((-currValToSend).ToString());
                currValToSend = 0;
            }
            yield return new WaitForSeconds(DELAY);
        }
    }
}
