using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject SectionMenu;
    public GameObject MenuButton;
    public AdjustHandle adjustHandle;

    public Slider occluderSlider;
    public Slider handleSlider;

    void Start() {
        CloseMenu();
        SetHandleSlider();
        SetOccluderSlider();
    }

    public void OpenMenu() {
        SectionMenu.SetActive(true);
        MenuButton.SetActive(false);
        adjustHandle.ActivateHandleVisualizer(true);
    }

    public void CloseMenu() {
        SectionMenu.SetActive(false);
        MenuButton.SetActive(true);
        adjustHandle.ActivateHandleVisualizer(false);
        adjustHandle.SaveChanges();
    }

    public void OnHandleSliderChanged(float val) {
        adjustHandle.ChangeHandle(val);
    }

    public void OnOccluderSliderChanged(float val) {
        adjustHandle.ChangeOccluder(val);
    }

    void SetHandleSlider() {
        AdjustmentInfo handleInfo = adjustHandle.GetHandleInfo();
        handleSlider.value = handleInfo.CurrValue;
        handleSlider.minValue = handleInfo.Min;
        handleSlider.maxValue = handleInfo.Max;
    }

    void SetOccluderSlider() {
        AdjustmentInfo occluderInfo = adjustHandle.GetOccluderInfo();
        occluderSlider.value = occluderInfo.CurrValue;
        occluderSlider.minValue = occluderInfo.Min;
        occluderSlider.maxValue = occluderInfo.Max;
    }
}
