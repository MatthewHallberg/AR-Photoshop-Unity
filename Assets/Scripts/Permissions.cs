using UnityEngine;
using UnityEngine.Android;

public class Permissions : MonoBehaviour {

    void Awake() {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
    }
}
