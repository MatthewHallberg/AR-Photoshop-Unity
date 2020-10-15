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

    void Awake() {
        //save values on first load
        if (!PlayerPrefs.HasKey(OCCLUDER_KEY)) {
            SaveChanges();
        }

        LoadSavedValues();
    }

    public void ChangeOccluder(float val) {
        Vector3 tempScale = Occluder.localScale;
        tempScale.y = val;
        Occluder.localScale = tempScale;
        occluderInfo.CurrValue = val;
        //move bottom distortion based on occluder scale
        float amount = Occluder.localScale.y + (distortBottom.localScale.y/2);
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

    public AdjustmentInfo GetHandleInfo() {
        return handleInfo;
    }

    public AdjustmentInfo GetOccluderInfo() {
        return occluderInfo;
    }

    public void ActivateHandleVisualizer(bool active) {
        occluderVisualizer.SetActive(active);
    }

    public void SaveChanges() {
        SaveOccluderWidth();
        SaveHandleHeight();
    }

    void LoadSavedValues() {
        ChangeOccluder(PlayerPrefs.GetFloat(OCCLUDER_KEY));
        ChangeHandle(PlayerPrefs.GetFloat(HANDLE_KEY));
    }

    void SaveOccluderWidth() {
        PlayerPrefs.SetFloat(OCCLUDER_KEY, occluderInfo.CurrValue);
    }

    void SaveHandleHeight() {
        PlayerPrefs.SetFloat(HANDLE_KEY, handleInfo.CurrValue);
    }
}
