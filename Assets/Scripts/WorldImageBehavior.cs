using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldImageBehavior : MonoBehaviour {

    const float ANIMATION_FRAME = .0412f;

    readonly Vector3 angleOffset = new Vector3(-90, 0, 180);

    public Transform imageParent;

    Camera mainCam;
    float camDistance;
    bool selected = true;
    bool isPlaying;
    float lastSliderVal;

    void Start() {
        mainCam = Camera.main;
        SelectItem(true);
    }

    void Update() {

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
            WorldImageManager.Instance.ItemSelected(this, isPlaying, lastSliderVal);
        }
    }

    public void RemoveItem() {
        Destroy(gameObject);
    }

    public void PlayAnimation(bool shouldPlay) {
        isPlaying = shouldPlay;
        if (shouldPlay) {
            StartCoroutine(PlayRoutine());
        } else {
            StopAllCoroutines();
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
                yield return new WaitForSecondsRealtime(ANIMATION_FRAME);
            }
        }
    }

    public void SetLayerDepth(float val) {
        lastSliderVal = val;
        foreach (Transform image in imageParent) {
            Vector3 pos = image.localPosition;
            pos.z = image.GetSiblingIndex() * val * -1f;
            image.localPosition = pos;
        }
    }
}
