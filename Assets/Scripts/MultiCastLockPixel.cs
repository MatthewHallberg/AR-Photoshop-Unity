using System.Collections;
using UnityEngine;

/// <summary>
/// Bugfix: Pixel 2 not recieving broadcast messages (sometimes??)
/// Ref: https://answers.unity.com/questions/250732/android-build-is-not-receiving-udp-broadcasts.html
/// </summary>

public class MultiCastLockPixel : MonoBehaviour {

    public static bool stopAcquiringLock;

    const float MULTICAST_DELAY = 1f;

    void Start() {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(acquireMultiCastPeriodically());
#endif
    }

    IEnumerator acquireMultiCastPeriodically() {
        while (!stopAcquiringLock) {
            bool gotLock = getMulticastLock("debugMulticast");
            if (gotLock) {
                Debug.Log("~~~~~ Got Multicast Lock ~~~~~~~");
                stopAcquiringLock = true;
            }
            yield return new WaitForSeconds(MULTICAST_DELAY);
        }
    }

    bool getMulticastLock(string lockTag) {
        try {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity")) {

                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi")) {
                    AndroidJavaObject multicastLock = wifiManager.Call<AndroidJavaObject>("createMulticastLock", lockTag);
                    multicastLock.Call("acquire");
                    bool isHeld = multicastLock.Call<bool>("isHeld");
                    return isHeld;
                }

            }
        } catch (System.Exception err) {
            Debug.Log(err.ToString());
        }
        return false;
    }
}