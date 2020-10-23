using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ClickHandler : MonoBehaviour {

    public UnityEvent OnClickEvent;

    void OnMouseUpAsButton() {
        OnClickEvent?.Invoke();
    }
}
