using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SinglePlayerLapControler : MonoBehaviour
{
    private List<GameObject> checkpoints = new List<GameObject>();
    //private List<GameObject> passedCheckpoints;

    private GameObject winPanel;
    private TMP_Text winPanelMessage;
    private GameObject losePanel;

    public static bool isFnished = false;

    private void Awake()
    {
        checkpoints.AddRange(SinglePlayerGameManager.instance.checkpoints);
        winPanel = SinglePlayerGameManager.instance.winPanel;
        winPanelMessage = SinglePlayerGameManager.instance.winPanelMessage;
        losePanel = SinglePlayerGameManager.instance.losePanel;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(checkpoints.Contains(other.gameObject))
        {
            int checkpointIndex = checkpoints.IndexOf(other.gameObject);
            checkpoints[checkpointIndex].SetActive(false);

            if (checkpointIndex == checkpoints.Count - 2)
            {
                checkpoints[checkpoints.Count - 1].SetActive(true);
            }
        }

        if (other.name == "FinishCheckpoint")
        {
            GameEnd();
        }
    }

    private void GameEnd()
    {
        isFnished = true;
        GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(false);

        if (SinglePlayerRaceTime.timeElapsed < SinglePlayerSetup.timeChallenge)
        {
            winPanelMessage.text = "Your Time: " + Math.Round(SinglePlayerRaceTime.timeElapsed, 2);
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
    }
}
