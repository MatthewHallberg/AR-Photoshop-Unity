using UnityEngine;

public class AdjustHandle : MonoBehaviour {

    const string OCCLUDER_KEY = "OccluderWidth";
    const string HANDLE_KEY = "HandleHeight";

    public Transform Occluder;

    void Awake() {
        //save values on first load
        if (!PlayerPrefs.HasKey(OCCLUDER_KEY)) {
            SaveChanges();
        }

        LoadSavedValues();
    }

    void SaveChanges() {
        SaveOccluderWidth();
        SaveHandleHeight();
    }

    void LoadSavedValues() {
        //set occluder width
        float occluderWidth = PlayerPrefs.GetFloat(OCCLUDER_KEY);
        Vector3 occluderSize = Occluder.localScale;
        occluderSize.y = occluderWidth;
        Occluder.localScale = occluderSize;

        //set handle height
        float handleHeight = PlayerPrefs.GetFloat(HANDLE_KEY);
        Vector3 handlePosition = transform.localPosition;
        handlePosition.z = handleHeight;
        transform.localPosition = handlePosition;
    }

    void SaveOccluderWidth() {
        PlayerPrefs.SetFloat(OCCLUDER_KEY, Occluder.localScale.y);
    }

    void SaveHandleHeight() {
        PlayerPrefs.SetFloat(HANDLE_KEY, transform.localPosition.z);
    }
}
