using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class LapController : MonoBehaviourPun
{
    private List<GameObject> lapCheckpoints = new List<GameObject>();

    public enum RaiseEventCode
    {
        WhoFinishedEventCode = 0
    }

    private bool passed = false;

    private int finishOrder = 0;
    private int got = 0;
    private int sent = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject lapCheckpoint in MultiplayerRacingModeManager.instance.lapCheckpoints)
        {
            lapCheckpoints.Add(lapCheckpoint);
        }
        lapCheckpoints[lapCheckpoints.Count - 1].SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        bool displayed = false;
        if (lapCheckpoints.Contains(other.gameObject))
        {
            int checkpointIndex = lapCheckpoints.IndexOf(other.gameObject);


            if (checkpointIndex == lapCheckpoints.Count - 2) // When it is the last checkpoint, activate the finish checkpoint
            {
                lapCheckpoints[lapCheckpoints.Count - 1].SetActive(true);
            }
            if (other.name == "FinishCheckpoint")
            {
                if (!passed)
                {
                    Debug.Log(gameObject.name);
                    if (!displayed)
                        GameFinished();
                    displayed = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {

        if (photonEvent.Code == (byte)RaiseEventCode.WhoFinishedEventCode)
        {
            got +=1;
            Debug.Log("Got " + got + " times");
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1]; // This is where other players updated their finish position
            int viewID = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUITextGameObject = MultiplayerRacingModeManager.instance.finishOrderUIGameObjects[finishOrder - 1];
            if (!orderUITextGameObject.activeSelf)
            {
                orderUITextGameObject.SetActive(true);
                if (viewID == photonView.ViewID)
                {
                    orderUITextGameObject.GetComponent<TMP_Text>().text = finishOrder + ". " + nickNameOfFinishedPlayer + " (YOU)";
                    orderUITextGameObject.GetComponent<TMP_Text>().color = Color.red;
                    MultiplayerRacingModeManager.instance.canvasPanel.SetActive(true);
                }
                else
                {
                    orderUITextGameObject.GetComponent<TMP_Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
                }
            }
        }
    }

    void GameFinished()
    {
        GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(false);
        Debug.Log("Sent " + ++sent + " times");

        finishOrder += 1;

        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        // event data
        Debug.Log("Nickname: " + nickName + ", FinishOrder: " + finishOrder + ", viewID: " + viewID);
        object[] data = new object[] { nickName, finishOrder, viewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
