using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovePhotoshop : MonoBehaviour {

    const float MAX_MOVE_AMOUNT = 8f;

    public Slider slider;
    float DELAY = .2f;

    float currDocValue = 0;
    float currValToSend;

    void Start() {
        SetUpSlider();
        StartCoroutine(SendMoveMessageRoutine());
    }

    void SetUpSlider() {
        slider.minValue = 0;
        slider.maxValue = MAX_MOVE_AMOUNT;
        slider.value = 0;
    }

    public void OnSliderValueChanged(float val) {
        float valToSend = val - currDocValue;
        currValToSend += valToSend;
        currDocValue = val;
    }

    IEnumerator SendMoveMessageRoutine() {
        while (true) {
            if (!currValToSend.Equals(0)) {
                ConnectionManager.Instance.SendUDP((-currValToSend).ToString());
                currValToSend = 0;
            }
            yield return new WaitForSeconds(DELAY);
        }
    }
}
