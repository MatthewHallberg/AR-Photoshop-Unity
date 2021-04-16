using UnityEngine;

public class AdjustHandle : MonoBehaviour {

    [Header("Handle")]
    const string HANDLE_KEY = "HandleHeight";
    public AdjustmentInfo handleInfo;
    public MoveHandle moveHandle;

    [Header("Occluder")]
    const string OCCLUDER_KEY = "OccluderWidth";
    public AdjustmentInfo occluderInfo;
    public Transform Occluder;
    public GameObject occluderVisualizer;
    public Transform distortTop;
    public Transform distortBottom;

    void Start() {
        //save values on first load
        if (!PlayerPrefs.HasKey(OCCLUDER_KEY)) {
            SaveChanges(occluderInfo.CurrValue, handleInfo.CurrValue);
        }

        LoadSavedValues();
    }

    public void ChangeOccluder(float val) {
        Vector3 tempScale = Occluder.localScale;
        tempScale.y = val;
        Occluder.localScale = tempScale;
        occluderInfo.CurrValue = val;
        //move bottom distortion based on occluder scale
        float amount = Occluder.localScale.y + (distortBottom.localScale.y / 2);
        Vector3 bottomPos = distortBottom.localPosition;
        bottomPos.z = Occluder.localPosition.z - amount;
        distortBottom.localPosition = bottomPos;
    }

    public void ChangeHandle(float val) {
        Vector3 tempPos = transform.localPosition;
        tempPos.z = val;
        transform.localPosition = tempPos;
        handleInfo.CurrValue = val;
        moveHandle.OnHandlePositionChanged();
    }

    public void ActivateHandleVisualizer(bool active) {
        occluderVisualizer.SetActive(active);
    }

    public void SaveChanges(float occluderWidth, float handleHeight) {
        SaveOccluderWidth(occluderWidth);
        SaveHandleHeight(handleHeight);
        PlayerPrefs.Save();
    }

    void LoadSavedValues() {
        ChangeOccluder(PlayerPrefs.GetFloat(OCCLUDER_KEY));
        ChangeHandle(PlayerPrefs.GetFloat(HANDLE_KEY));
        Menu.Instance.SetHandleSlider(handleInfo);
        Menu.Instance.SetOccluderSlider(occluderInfo);
    }

    void SaveOccluderWidth(float width) {
        PlayerPrefs.SetFloat(OCCLUDER_KEY, width);
    }

    void SaveHandleHeight(float height) {
        PlayerPrefs.SetFloat(HANDLE_KEY, height);
    }
}
