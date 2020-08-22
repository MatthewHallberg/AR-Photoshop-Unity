using UnityEngine;

public class MovePhotoshop : MonoBehaviour {

    public void MoveUp() {
        ConnectionManager.Instance.SendUDP("-.5");
    }

    public void MoveDown() {
        ConnectionManager.Instance.SendUDP(".5");
    }
}
