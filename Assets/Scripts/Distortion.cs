using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Distortion : MonoBehaviour {

    const float RANGE = 2f;
    const float SPEED = 20;

    float currDistortion;
    Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        float currValue = Mathf.Sin(Time.time * SPEED) * currDistortion;
        rend.material.SetFloat("_DistortionAmount", currValue);
    }

    public void SetDistortion(bool active) {
        currDistortion = active ? RANGE : 0;
    }
}
