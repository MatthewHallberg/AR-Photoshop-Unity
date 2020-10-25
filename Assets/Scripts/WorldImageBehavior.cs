using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldImageBehavior : MonoBehaviour {

    const float ROTATE_SPEED = 6f;

    readonly Vector3 angleOffset = new Vector3(-90, 0, 180);

    public Transform imageParent;

    Camera mainCam;
    float camDistance;
    bool selected = true;

    Quaternion desiredRot;

    void Start() {
        mainCam = Camera.main;
        SelectItem(true);
    }

    void Update() {
        if (!selected) {
            return;
        }

        //handle position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camDistance;
        transform.position = mainCam.ScreenToWorldPoint(mousePos);
        //handle rotation
        transform.LookAt(mainCam.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles += angleOffset);

        //interpolate angle
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * ROTATE_SPEED);

        if (Input.GetMouseButtonUp(0)) {
            SelectItem(false);
        }
    }

    void OnMouseDown() {
        SelectItem(true);
    }

    void SelectItem(bool selectedState) {
        selected = selectedState;
        //set parent to avoid rotation when moving in AR
        transform.parent = selectedState ? mainCam.transform : null;
        camDistance = Vector3.Distance(transform.position, mainCam.transform.position);
    }

    public void RemoveItem() {
        Destroy(gameObject);
    }
}
