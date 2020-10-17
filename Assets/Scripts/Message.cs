using TMPro;
using UnityEngine;

public class Message : Singleton<Message> {

    public TextMeshProUGUI messageText;

    public void ShowMessage(string msg) {
        messageText.text = msg;
        transform.localScale = Vector3.one;
    }

    public void CloseMessage() {
        transform.localScale = Vector3.zero;
    }
}
