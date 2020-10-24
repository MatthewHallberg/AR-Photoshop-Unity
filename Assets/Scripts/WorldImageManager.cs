using UnityEngine;

public class WorldImageManager : Singleton<WorldImageManager> {

    readonly Vector3 angleOffset = new Vector3(-90, 0, 180);

    public GameObject imagePrefab;
    public GameObject holderPrefab;

    Camera mainCam;
    float camDistance;

    void Start() {
        mainCam = Camera.main;
    }

    public void Create() {
        Transform handle = MoveHandle.Instance.transform;
        GameObject worldSpaceImages = Instantiate(holderPrefab, handle.position,handle.rotation);
        WorldImageBehavior worldImage = worldSpaceImages.GetComponent<WorldImageBehavior>();
        foreach (Transform img in CreateImage.Instance.transform) {
            GameObject image = Instantiate(img.gameObject, worldImage.imageParent);
        }
    }

    void Update() {
        if (Input.GetMouseButton(0)) {

            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("WorldImage")) {
     
                    Transform image = hit.collider.transform;

                    if (camDistance.Equals(0)) {
                        camDistance = Vector3.Distance(image.position, mainCam.transform.position);
                    }

                    //handle position
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = camDistance;
                    image.position = mainCam.ScreenToWorldPoint(mousePos);
                    //handle rotation
                    image.LookAt(mainCam.transform);
                    image.eulerAngles += angleOffset;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            camDistance = 0;
        }
    }
}
