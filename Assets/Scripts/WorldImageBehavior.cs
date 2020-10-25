using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldImageBehavior : MonoBehaviour {

    const float SPEED = 6f;

    readonly Vector3 angleOffset = new Vector3(-90, 0, 180);

    public Transform imageParent;
    public GameObject imgStop;
    public Transform uiParent;
    public Canvas sliderCanvas;

    Camera mainCam;
    float camDistance;
    bool selected = true;
    Vector3 desiredUIScale;

    Quaternion desiredRot;

    void Start() {
        mainCam = Camera.main;
        sliderCanvas.worldCamera = mainCam;
        uiParent.localScale = desiredUIScale;
        SelectItem(true);
    }

    void Update() {

        uiParent.localScale = Vector3.Lerp(uiParent.localScale, desiredUIScale, Time.deltaTime * SPEED);

        if (!selected) {
            return;
        }

        //handle position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camDistance;
        transform.position = mainCam.ScreenToWorldPoint(mousePos);
        //handle rotation
        transform.LookAt(mainCam.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles += angleOffset);

        //interpolate angle
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * SPEED);

        if (Input.GetMouseButtonUp(0)) {
            SelectItem(false);
        }
    }

    void OnMouseDown() {
        SelectItem(true);
    }

    public void SelectItem(bool selectedState) {
        selected = selectedState;
        //set parent to avoid rotation when moving in AR
        transform.parent = selectedState ? mainCam.transform : null;
        camDistance = Vector3.Distance(transform.position, mainCam.transform.position);

        if (selected) {
            WorldImageManager.Instance.ActivateUI(this);
        }
    }

    public void ActivateUI(bool active) {
        desiredUIScale = active ? Vector3.one * 10 : Vector3.zero;
    }

    public void RemoveItem() {
        Destroy(gameObject);
    }

    public void PlayButtonDown() {
        StopAllCoroutines();
        imgStop.SetActive(!imgStop.activeSelf);
        bool isPlaying = imgStop.activeSelf;
        if (isPlaying) {
            StartCoroutine(PlayRoutine());
        } else {
            ActivateAllImages(true);
        }
    }

    void ActivateAllImages(bool active) {
        foreach (Transform image in imageParent) {
            image.gameObject.SetActive(active);
        }
    }

    IEnumerator PlayRoutine() {
        while (true) {
            foreach (Transform image in imageParent) {
                ActivateAllImages(false);
                image.gameObject.SetActive(true);
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    public void OnSliderValueChanged(float val) {
        foreach (Transform image in imageParent) {
            Vector3 pos = image.localPosition;
            pos.z = image.GetSiblingIndex() * val * -1f;
            image.localPosition = pos;
        }
    }
}
