using System.Collections;
using UnityEngine;

public class MoveHandle : Singleton<MoveHandle> {

    const float MOVE_SPEED = 15;

    public Transform ImageParent;
    public GameObject ActivateButton;
    public Transform TransParent;
    public Transform TransparentImages;
    public Transform HandleImage;
    public DistortionController distortion;

    float documentHeight = 1;
    Vector3 imageTopPos;
    Vector3 imageBottomPos;
    Vector3 desiredImagePos;
    Vector3 desiredHandleScale = new Vector3(.3f, .3f, .3f);

    Camera mainCam;
    Vector3 startPosition;
    bool shouldMoveHandle = true;
    bool detached;

    protected override void Awake() {
        base.Awake();
        mainCam = Camera.main;
        ResetImagePositions();
        EnableHandleVisuals(true);
    }

    void Update() {

        ImageParent.localPosition = Vector3.Lerp(ImageParent.localPosition, desiredImagePos, Time.deltaTime * MOVE_SPEED);
        HandleImage.localScale = Vector3.Lerp(HandleImage.localScale, desiredHandleScale, Time.deltaTime * MOVE_SPEED);

        if (detached) {
            return;
        }

        if (shouldMoveHandle) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * MOVE_SPEED);
            if (Vector3.Distance(transform.localPosition, startPosition) < .1) {
                transform.localPosition = startPosition;
                desiredImagePos = imageBottomPos;
                distortion.ActivateDistortion(false);
            }
        }
    }

    void OnMouseDown() {
        shouldMoveHandle = false;
        desiredImagePos = imageTopPos;
        desiredHandleScale.x = .2f;
        distortion.ActivateDistortion(true);

        EnableImageVisuals(true);
    }

    void OnMouseUp() {
        shouldMoveHandle = true;
        desiredHandleScale.x = .3f;
    }

    void OnMouseDrag() {

        if (shouldMoveHandle) {
            return;
        }

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

        if (localPos.z >= startPosition.z && localPos.z <= startPosition.z + documentHeight) {
            transform.localPosition = currPos;
        } else if (localPos.z > startPosition.z + documentHeight) {
            DetachImage();
        }
    }

    void DetachImage() {
        Debug.Log("Detaching");
        detached = true;
        shouldMoveHandle = true;
        ImageParent.gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        EnableHandleVisuals(false);
        WorldImageManager.Instance.Create();
        desiredImagePos = imageBottomPos;
        transform.localPosition = startPosition;
        distortion.ActivateDistortion(false);
    }

    void ResetImagePositions() {
        startPosition = transform.localPosition;
        SetDocumentHeight();
        ImageParent.position = TransparentImages.position;
        imageBottomPos = ImageParent.localPosition;
        desiredImagePos = imageBottomPos;
    }

    public void ReactivateImage() {
        detached = false;
        shouldMoveHandle = false;
        ImageParent.gameObject.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        EnableHandleVisuals(true);
        WorldImageManager.Instance.OpenMenu(false);
        SetDocumentHeight();
    }

    void SetDocumentHeight() {
        documentHeight = ImageParent.localScale.y;
        float startZPos = (documentHeight + .1f) * -1f;
        imageTopPos = new Vector3(0, 0, startZPos);
    }
     
    public void EnableImageVisuals(bool active) {
        ImageParent.gameObject.SetActive(active);
        TransParent.gameObject.SetActive(active);
    }

    public void EnableHandleVisuals(bool active) {
        HandleImage.gameObject.SetActive(active);
        ActivateButton.SetActive(!active);
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
