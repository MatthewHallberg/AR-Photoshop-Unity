using UnityEngine;

public class Menu : MonoBehaviour {

    public GameObject SectionMenu;
    public GameObject MenuButton;
    public AdjustHandle adjustHandle;

    void Start() {
        CloseMenu();
    }

    public void OpenMenu() {
        SectionMenu.SetActive(true);
        MenuButton.SetActive(false);
    }

    public void CloseMenu() {
        SectionMenu.SetActive(false);
        MenuButton.SetActive(true);
    }

    public void OnHandleSliderChanged(float val) {

    }

    public void OnOccluderSliderChanged(float val) {

    }
}
