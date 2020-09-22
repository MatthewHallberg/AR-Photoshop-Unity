using System.Collections;
using UnityEngine;

public class MovePhotoshop : MonoBehaviour {

    const float MOVE_SPEED = 6f;
    const float MAX_PHOTOSHOP_MOVE = 7.5f;
    const float UNITY_DOCUMENT_HANDLE_HEIGHT = 2f;

    readonly float DELAY = .2f;

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
        float distance = transform.position.z - mainCam.transform.position.z;
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 worldTouchPos = Camera.main.ScreenToWorldPoint(mousePos);

        //add axis to current position
        Vector3 currPosition = transform.position;
        currPosition.y = worldTouchPos.y;

        //convert to localPosition
        Vector3 localPos = transform.parent.InverseTransformPoint(currPosition);

        if (localPos.y >= startPosition.y && localPos.y <= startPosition.y + UNITY_DOCUMENT_HANDLE_HEIGHT) {
            transform.position = currPosition;
        }
    }

    void CalcPhotoshopPosition() {
        //remap handle position to photoshop coordinates
        float currVal = transform.localPosition.y;
        float from1 = startPosition.y;
        float to1 = startPosition.y + UNITY_DOCUMENT_HANDLE_HEIGHT;
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
