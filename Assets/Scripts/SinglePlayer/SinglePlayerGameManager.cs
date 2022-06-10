using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerGameManager : MonoBehaviour
{
    public static SinglePlayerGameManager instance;

    public GameObject[] PlayerPrefabs;
    public Transform startPositionTransform;
    
    public TMP_Text timeCountDownText;
    public TMP_Text time;

    public List<GameObject> checkpoints = new List<GameObject>();
    
    public GameObject winPanel;
    public TMP_Text winPanelMessage;
    public GameObject losePanel;

    public GameObject obstacles;
    public GameObject moreObstacles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }


        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameLoader"))
        {
            Destroy(GameObject.Find("GameLoader")); // Destroy the loader since we don't need it
        }
        switch (SinglePlayerSetup.difficultyLevel)
        {
            case 3:
                obstacles.SetActive(true); // Bring obstacles at level 2
                moreObstacles.SetActive(true); // Also level 3
                break;
            case 2:
                obstacles.SetActive(true); // Bring obstacles at level 2
                moreObstacles.SetActive(false);
                break;
            default:
                // Do nothing, at difficult level 1
                obstacles.SetActive(false);
                moreObstacles.SetActive(false);
                break;
        }
        Instantiate(PlayerPrefabs[SinglePlayerSetup.kartNumber], startPositionTransform.position, startPositionTransform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToGameMenu()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void RetryGame()
    {
        // This won't work due to singleton implementation
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("This feature will be implemented later");
    }

}
