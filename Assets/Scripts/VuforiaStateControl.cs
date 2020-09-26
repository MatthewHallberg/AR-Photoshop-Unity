using System.Collections;
using UnityEngine;
using Vuforia;

public class VuforiaStateControl : MonoBehaviour {

    void OnDisable() {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
    }

    void OnEnable() {
        VuforiaRuntimeUtilities.SetAllowedFusionProviders(FusionProviderType.VUFORIA_SENSOR_FUSION);
        Debug.Log("Setting Vuforia Fusion provider to Sensor Fusion.");
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
    }

    void OnVuforiaStarted() {
        StartCoroutine(SettingsRoutine());
    }

    IEnumerator SettingsRoutine() {
        yield return new WaitForSeconds(0.5f);
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
}