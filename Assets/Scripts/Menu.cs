using UnityEngine;
using UnityEngine.UI;

public class Menu : Singleton<Menu> {

    public GameObject SectionMenu;
    public GameObject MenuButton;
    public AdjustHandle adjustHandle;

    public Slider occluderSlider;
    public Slider handleSlider;

    void Start() {
        ActivateMenuVisuals(false);
    }

    public void OpenMenu() {
        ActivateMenuVisuals(true);
        WorldImageManager.Instance.OpenMenu(false);
    }

    public void CloseMenu() {
        ActivateMenuVisuals(false);
        adjustHandle.SaveChanges(occluderSlider.value, handleSlider.value);
    }

    public void OnHandleSliderChanged(float val) {
        adjustHandle.ChangeHandle(val);
    }

    public void OnOccluderSliderChanged(float val) {
        adjustHandle.ChangeOccluder(val);
    }

    public void SetHandleSlider(AdjustmentInfo handleInfo) {
        handleSlider.value = handleInfo.CurrValue;
        handleSlider.minValue = handleInfo.Min;
        handleSlider.maxValue = handleInfo.Max;
    }

    public void SetOccluderSlider(AdjustmentInfo occluderInfo) {
        occluderSlider.value = occluderInfo.CurrValue;
        occluderSlider.minValue = occluderInfo.Min;
        occluderSlider.maxValue = occluderInfo.Max;
    }

    void ActivateMenuVisuals(bool active) {
        SectionMenu.SetActive(active);
        adjustHandle.ActivateHandleVisualizer(active);
        MenuButton.SetActive(!active);
    }
}
