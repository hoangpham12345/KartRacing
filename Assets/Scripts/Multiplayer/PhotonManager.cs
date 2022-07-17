using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayers = 4;

    private string roomMessage;
    private string roomID;

    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    // Panels to manipulate between lobby and room
    public GameObject multiplayerLobby;
    public GameObject multiplayerRoom;

    private string roomInfo;

    // Room prefab and its container in the lobby
    public GameObject roomListPrefab;
    public GameObject roomListContainer;

    // Player prefab and its container in the room
    public GameObject playerListPrefab;
    public GameObject playerListContainer;

    public GameObject nextMapButton;
    public GameObject previousMapButton;
    public GameObject[] selectableMaps;

    public GameObject startGameButton;

    private Dictionary<int, GameObject> playerListGameObjects;

    //public void Start()
    //{
    //    PhotonNetwork.AutomaticallySyncScene = true;
    //}

    public static void ConnectToPhotonService(string nickName)
    {
        Debug.Log("Try to connect to Photon service");
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.LocalPlayer.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    public void DisconnectFromPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Try to disconnect from Photon service");
            PhotonNetwork.Disconnect();
            multiplayerLobby.SetActive(false);
        }
    }

    #region Lobby

    public override void OnConnected()
    {
        Debug.Log("We connected to the Internet");
    }

    public override void OnConnectedToMaster()
    {
        // base.OnConnectedToMaster();
        Debug.Log("Connected to Photon service " + PhotonNetwork.LocalPlayer.ToStringFull());
        JoinLobby();
    }

    public void JoinLobby()
    {
        Debug.Log("Try to join the lobby");
        PhotonNetwork.JoinLobby(customLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        cachedRoomList.Clear();
        multiplayerLobby.SetActive(true);
        multiplayerRoom.SetActive(false);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        Debug.Log("Update cached room list");
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        DisplayRoomList();
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public void DisplayRoomList()
    {
        Debug.Log("Display room list");

        // Delete old instances of the roomListPrefabs
        foreach(Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Then create new instances of roomListPrefabs
        foreach(RoomInfo roomInfo in cachedRoomList.Values)
        {
            string id = roomInfo.Name;
            //string message = roomInfo.CustomProperties.TryGetValue("message");
            object messageObject;
            string message = "";
            if (roomInfo.CustomProperties.TryGetValue("msg", out messageObject)){
                message = (string) messageObject;
            }
            string players = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
            bool isOpen = roomInfo.IsOpen;

            GameObject roomListGO = Instantiate(roomListPrefab);
            roomListGO.transform.SetParent(roomListContainer.transform);
            roomListGO.transform.localScale = Vector3.one;
            roomListGO.GetComponent<RoomListInitializer>().Initialize(id, message, players);
            if (isOpen)
            {
                roomListGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    PhotonNetwork.JoinRoom(id);
                });
            }
            else
            {
                roomListGO.GetComponent<Button>().interactable = false;
            }
        }
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
        Debug.Log("Disconnected to Photon service.");
        if (!(multiplayerLobby is null))
            multiplayerLobby.SetActive(false);
        if (!(multiplayerRoom is null))
            multiplayerRoom.SetActive(false);
    }
    #endregion

    #region Room manupulation
    public void SetRoomMessage(string roomMessage)
    {
        this.roomMessage = roomMessage;
    }

    public void SetRoomID(string roomID)
    {
        this.roomID = roomID;
    }

    public void CreateNewRoom()
    {
        if (string.IsNullOrEmpty(roomMessage))
        {
            // default roomMessage
            roomMessage = "Come and play!";
        }
        System.Random random = new System.Random();
        string roomID = random.Next(0, 100).ToString();
        string mapIndex = 0.ToString();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        string[] roomPropsInLobby = { "msg" };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {{ "msg", roomMessage }, {"m", mapIndex } };

        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;

        PhotonNetwork.CreateRoom(roomID, roomOptions);
        roomMessage = "";
    }

    public void JoinRoomWithID()
    {
        if (string.IsNullOrEmpty(roomID))
        {
            Debug.Log("You must enter the roomID");
        } else
        {
            PhotonNetwork.JoinRoom(roomID);
        }

    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.AutomaticallySyncScene)
            PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " " + "joined to " + PhotonNetwork.CurrentRoom.Name);
        roomInfo = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        multiplayerLobby.SetActive(false);
        multiplayerRoom.SetActive(true);

        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = roomInfo;

        // Update Player list

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab);
            playerListGameObject.transform.SetParent(playerListContainer.transform);
            playerListGameObject.transform.localScale = Vector3.one;
            playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);

            ExitGames.Client.Photon.Hashtable playerCustomProperties = player.CustomProperties;
            object isPlayerReady;
            if (playerCustomProperties.TryGetValue(MultiplayerPlayerConfig.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
        }

        startGameButton.SetActive(false);
        previousMapButton.SetActive(false);
        nextMapButton.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            previousMapButton.SetActive(true);
            nextMapButton.SetActive(true);
        }

        // Update map
        object mapIndex;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out mapIndex))
        {
            //Debug.Log("mapIndex " + mapIndex);
            ActivateMap(Convert.ToInt32(mapIndex));
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        // We will create new room with default message
        CreateNewRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // Create new player game object
        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContainer.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(MultiplayerPlayerConfig.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnLeftRoom()
    {
        //BackToLobby();
        if (PhotonNetwork.AutomaticallySyncScene)
            PhotonNetwork.AutomaticallySyncScene = false;

        multiplayerLobby.SetActive(true);
        multiplayerRoom.SetActive(false);
        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.SetActive(CheckPlayersReady());
            previousMapButton.SetActive(true);
            nextMapButton.SetActive(true);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        object mapIndex;
        if (changedProps.TryGetValue("map", out mapIndex))
        {
            Debug.Log("Is map value updated? " + mapIndex);
            ActivateMap(Convert.ToInt32(mapIndex));
        }
    }

    private void ActivateMap(int x)
    {
        foreach (GameObject selectableMap in selectableMaps)
        {
            selectableMap.SetActive(false);
        }

        selectableMaps[x].SetActive(true);

    }
    #endregion


    public void BackToLobby()
    { 
        PhotonNetwork.LeaveRoom();

    }

    public void StartMultiplayerGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        object mapIndex;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m", out mapIndex))
        {
            switch (Convert.ToInt32(mapIndex))
            {
                case 0:
                    PhotonNetwork.LoadLevel("Scenes/Multiplayer/Map1");
                    break;
                case 1:
                    PhotonNetwork.LoadLevel("Scenes/Multiplayer/Map2");
                    break;
            }
        }
    }

    #region Supplemental code
    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    private bool CheckPlayersReady()
    {

        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(MultiplayerPlayerConfig.PLAYER_READY, out isPlayerReady))
            {

                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    #endregion

}
