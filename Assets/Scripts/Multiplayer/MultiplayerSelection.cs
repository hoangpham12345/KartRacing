using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerSelection : MonoBehaviour
{
    public GameObject[] selectableKarts;
    public GameObject[] selectableMaps;

    public int playerSelectionNumber;
    public int mapNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerSelectionNumber = 0;
        mapNumber = 0;

        ActivateKart(playerSelectionNumber);

        if (GameObject.Find("MultiplayerGameManager"))
        {
            Destroy(GameObject.Find("MultiplayerGameManager"));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ActivateKart(int x)
    {
        foreach (GameObject selectableKart in selectableKarts)
        {
            selectableKart.SetActive(false);
        }

        selectableKarts[x].SetActive(true);

        //Debug.Log("New kart index:" + x);

        //setting up player selection custom property
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerPlayerConfig.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

    }

    public void NextKart()
    {
        playerSelectionNumber += 1;
        if (playerSelectionNumber >= selectableKarts.Length)
        {
            playerSelectionNumber = 0;
        }
        ActivateKart(playerSelectionNumber);
    }

    public void PreviousKart()
    {

        playerSelectionNumber -= 1;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = selectableKarts.Length - 1;
        }

        ActivateKart(playerSelectionNumber);

    }

    public void NextMap()
    {
        mapNumber += 1;
        if (mapNumber >= selectableMaps.Length)
        {
            mapNumber = 0;
        }
        ExitGames.Client.Photon.Hashtable newMapProperties = new ExitGames.Client.Photon.Hashtable() { { "map", mapNumber } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newMapProperties);
    }

    public void PreviousMap()
    {
        mapNumber -= 1;
        if (mapNumber < 0)
        {
            mapNumber = selectableMaps.Length - 1;
        }
        ExitGames.Client.Photon.Hashtable newMapProperties = new ExitGames.Client.Photon.Hashtable() { { "map", mapNumber } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newMapProperties);
    }

}
