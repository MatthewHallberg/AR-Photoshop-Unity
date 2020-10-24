using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldImageBehavior : MonoBehaviour {

    public Transform imageParent;

    public void RemoveItem() {
        Destroy(gameObject);
    }
}
