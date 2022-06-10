using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomListInitializer : MonoBehaviour
{
    public TMP_Text id;
    public TMP_Text message;
    public TMP_Text players;

    public void Initialize(string id, string message, string players)
    {
        this.id.text = id;
        this.message.text = message;
        this.players.text = players;
    }
}
