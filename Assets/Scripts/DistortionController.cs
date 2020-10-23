using System.Collections.Generic;
using UnityEngine;

public class DistortionController : MonoBehaviour {

    public List<Distortion> distortionElements = new List<Distortion>();

    public void ActivateDistortion(bool active) {
        foreach (Distortion distort in distortionElements) {
            distort.SetDistortion(active);
        }
    }
}
