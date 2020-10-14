using System.Collections;
using UnityEngine;

public class MoveHandle : MonoBehaviour {

    const float MOVE_SPEED = 15;
    const float DOCUMENT_HEIGHT = 1f;

    public Transform ImageParent;
    public Transform TransparentImages;
    public Transform HandleImage;
    public DistortionController distortion;

    Vector3 imageTopPos;
    Vector3 imageBottomPos;
    Vector3 desiredImagePos;
    Vector3 desiredImageScale = new Vector3(.3f, .3f, .3f);

    Camera mainCam;
    Vector3 startPosition;
    bool shouldMoveHandle = true;

    void Awake() {
        mainCam = Camera.main;
        ResetImagePositions();
    }

    void Update() {
        if (shouldMoveHandle) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * MOVE_SPEED);
            if (Vector3.Distance(transform.localPosition, startPosition) < .1) {
                transform.localPosition = startPosition;
                desiredImagePos = imageBottomPos;
                distortion.ActivateDistortion(false);
            }
        }

        ImageParent.localPosition = Vector3.Lerp(ImageParent.localPosition, desiredImagePos, Time.deltaTime * MOVE_SPEED);
        HandleImage.localScale = Vector3.Lerp(HandleImage.localScale, desiredImageScale, Time.deltaTime * MOVE_SPEED);
    }

    void OnMouseDown() {
        shouldMoveHandle = false;
        desiredImagePos = imageTopPos;
        desiredImageScale.x = .2f;
        distortion.ActivateDistortion(true);
    }

    void OnMouseUp() {
        shouldMoveHandle = true;
        desiredImageScale.x = .3f;
    }

    void OnMouseDrag() {

        //get world position of screen touch
        Transform imageTarget = transform.parent;
        float distance = Vector3.Distance(imageTarget.position, mainCam.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 worldTouchPos = mainCam.ScreenToWorldPoint(mousePos);

        //convert to localPosition
        Vector3 localPos = transform.parent.InverseTransformPoint(worldTouchPos);

        //only allow handle to move on local z axis (up and down)
        Vector3 currPos = startPosition;
        currPos.z = localPos.z;

        if (localPos.z >= startPosition.z && localPos.z <= startPosition.z + DOCUMENT_HEIGHT) {
            transform.localPosition = currPos;
        }
    }

    void ResetImagePositions() {
        startPosition = transform.localPosition;
        imageTopPos = ImageParent.localPosition;
        ImageParent.position = TransparentImages.position;
        imageBottomPos = ImageParent.localPosition;
        desiredImagePos = imageBottomPos;
    }

    public void OnHandlePositionChanged() {
        StartCoroutine(ChangeHandleRoutine());
    }

    IEnumerator ChangeHandleRoutine() {
        yield return new WaitForEndOfFrame();
        ImageParent.position = TransparentImages.position;
        imageBottomPos = ImageParent.localPosition;
        desiredImagePos = imageBottomPos;
    }
}
