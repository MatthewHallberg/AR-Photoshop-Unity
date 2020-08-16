using UnityEngine;

[RequireComponent(typeof(SendMessage))]
public class MovePhotoshop : MonoBehaviour {

    SendMessage sendMessage;

    void Start() {
        sendMessage = GetComponent<SendMessage>();
    }

    public void MoveUp() {
        sendMessage.SendPacket("-.5");
    }

    public void MoveDown() {
        sendMessage.SendPacket(".5");
    }
}
