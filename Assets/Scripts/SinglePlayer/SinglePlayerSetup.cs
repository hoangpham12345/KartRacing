using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class SinglePlayerSetup : MonoBehaviour
{
    public GameObject[] selectableKarts;
    public GameObject[] selectableMaps;
    //public GameObject[] SelectableDifficulty;
    public ToggleGroup difficultyToggleGroup;

    public static int kartNumber;
    public static int mapNumber;
    public static int difficultyLevel;
    public static int timeChallenge;

    public TMP_Text tipText;

    // Start is called before the first frame update
    void Start()
    {
        // Set default values
        kartNumber = 0;
        mapNumber = 0;
        difficultyLevel = 1;
        timeChallenge = 100;

        ActivateKart(kartNumber);
        ActivateMap(mapNumber);

        if (GameObject.Find("SinglePlayerGameManager"))
        {
            Destroy(GameObject.Find("SinglePlayerGameManager")); 
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

    }

    public void NextKart()
    {
        kartNumber += 1;
        if (kartNumber >= selectableKarts.Length)
        {
            kartNumber = 0;
        }
        ActivateKart(kartNumber);
    }

    public void PreviousKart()
    {
        kartNumber -= 1;
        if (kartNumber < 0)
        {
            kartNumber = selectableKarts.Length - 1;
        }
    }

    private void ActivateMap(int x)
    {
        foreach (GameObject selectableMap in selectableMaps)
        {
            selectableMap.SetActive(false);
        }

        selectableMaps[x].SetActive(true);

        SetTimeChallenge();
    }

    public void NextMap()
    {
        mapNumber += 1;
        if (mapNumber >= selectableMaps.Length)
        {
            mapNumber = 0;
        }
        ActivateMap(mapNumber);
    }

    public void PreviousMap()
    {
        mapNumber -= 1;
        if (mapNumber < 0)
        {
            mapNumber = selectableMaps.Length - 1;
        }
        ActivateMap(mapNumber);
    }

    public void SetDifficulty()
    {
        Toggle selectedToggle = difficultyToggleGroup.ActiveToggles().FirstOrDefault();
        string selectedToggleName = selectedToggle.name;
        difficultyLevel = int.Parse(selectedToggleName);

        Debug.Log("Difficulty switched to: " + difficultyLevel);

        SetTimeChallenge();
    }

    public void StartSinglePlayerGame()
    {
        int mapToLoad = mapNumber + 1;

        StartCoroutine(GameLoader.instance.LoadLevelAsynchronously("Scenes/SinglePlayer/Map" + mapToLoad));
    }

    public void SetTimeChallenge()
    {
        switch (mapNumber)
        {
            case 0:
                switch (difficultyLevel)
                {
                    case 3:
                        timeChallenge = 30;
                        tipText.text = "Finish the race in 30s";
                        break;
                    case 2:
                        timeChallenge = 35;
                        tipText.text = "Finish the race in 35s";
                        break;
                    default:
                        timeChallenge = 40;
                        tipText.text = "Finish the race in 40s";
                        break;
                }
                break;
            case 1:
                switch (difficultyLevel)
                {
                    case 3:
                        timeChallenge = 55;
                        tipText.text = "Finish the race in 55s";
                        break;
                    case 2:
                        timeChallenge = 65;
                        tipText.text = "Finish the race in 65s";
                        break;
                    default:
                        timeChallenge = 75;
                        tipText.text = "Finish the race in 70s";
                        break;
                }
                break;
        }
    }
}