using UnityEngine;
using UnityEngine.UI;

public class WorldImageManager : Singleton<WorldImageManager> {

    const float MAX_SLIDER_VALUE = .30f;

    public GameObject imagePrefab;
    public GameObject holderPrefab;
    public GameObject imgStop;
    public Slider slider;

    Vector3 desiredScale;
    float moveSpeed = 6f;
    WorldImageBehavior currItem;

    void Start() {
        slider.maxValue = MAX_SLIDER_VALUE;
        imgStop.SetActive(false);
        transform.localScale = desiredScale;
    }

    void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * moveSpeed);
    }

    public void Create() {
        OpenMenu(false);
        Transform handle = MoveHandle.Instance.transform;
        GameObject worldSpaceImages = Instantiate(holderPrefab, handle.position, handle.rotation);
        WorldImageBehavior worldImage = worldSpaceImages.GetComponent<WorldImageBehavior>();
        foreach (Transform img in CreateImage.Instance.transform) {
            GameObject image = Instantiate(img.gameObject, worldImage.imageParent.transform);
            image.transform.SetAsFirstSibling();
        }
    }

    public void ItemSelected(WorldImageBehavior currWorldImage, bool isPlaying, float sliderVal) {
        currItem = currWorldImage;
        imgStop.SetActive(isPlaying);
        slider.value = sliderVal;
        OpenMenu(true);
    }

    public void OnSliderValueChanged(float val) {
        if (currItem != null) {
            currItem.SetLayerDepth(val);
        }
    }

    public void PlayButtonDown() {
        if (currItem != null) {
            imgStop.SetActive(!imgStop.activeSelf);
            bool isPlaying = imgStop.activeSelf;
            currItem.PlayAnimation(isPlaying);
        }
    }

    public void RemoveButtonDown() {
        if (currItem != null) {
            currItem.RemoveItem();
            OpenMenu(false);
        }
    }

    public void OpenMenu(bool open) {
        desiredScale = open ? Vector3.one : Vector3.zero;
        moveSpeed = open ? 7 : 15;
        if (!open) currItem = null;
    }
}
