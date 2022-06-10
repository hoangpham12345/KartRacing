using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class MultiplayerRacingModeManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public Transform[] InstantiatePositions;

    public TMP_Text timeCountDownText;
    public GameObject[] finishOrderUIGameObjects;
    public GameObject canvasPanel;

    public List<GameObject> lapCheckpoints = new List<GameObject>();

    //Singeleton Implementation
    public static MultiplayerRacingModeManager instance = null;

    private void Awake()
    {
        //check if instance already exists
        if (instance == null)
        {
            instance = this;
        }

        //If intance already exists and it is not !this!
        else if (instance != this)
        {
            //Then, destroy this. This enforces our singletton pattern, meaning rhat there can only ever be one instance of a GameManager
            Destroy(gameObject);

        }

        //To not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerPlayerConfig.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = InstantiatePositions[actorNumber - 1].position;
                Quaternion rotationPosition = InstantiatePositions[actorNumber - 1].rotation;

                PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, rotationPosition);

            }

        }

        foreach (GameObject o in finishOrderUIGameObjects)
        {
            o.SetActive(false);
        }

        if (PhotonNetwork.AutomaticallySyncScene)
            PhotonNetwork.AutomaticallySyncScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToGameMenu()
    {
        if (PhotonNetwork.AutomaticallySyncScene)
            PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync("MainScene");
    }
}
