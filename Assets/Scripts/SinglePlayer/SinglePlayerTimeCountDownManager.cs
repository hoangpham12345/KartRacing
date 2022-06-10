using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SinglePlayerTimeCountDownManager : MonoBehaviour
{
    private TMP_Text timeCountDownText;
    private float timeToStartRace = 3.0f;
    private int oldTimeInt = 4;

    private void Awake()
    {
        timeCountDownText = SinglePlayerGameManager.instance.timeCountDownText;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToStartRace >= 0.0f)
        {
            timeToStartRace -= Time.deltaTime;

            int timeInt = (int)Math.Ceiling(timeToStartRace);

            if (oldTimeInt != timeInt)
            {
                SetCountDownTime(timeInt);
                oldTimeInt = timeInt;
            }
        }
        else if (timeToStartRace < 0.0f)
        {
            // Player is allowed to move
            GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(true);
            GetComponent<SinglePlayerRaceTime>().enabled = true;
            this.enabled = false;
            
        }
    }

    public void SetCountDownTime(int timeInt)
    {
        if (timeInt > 0)
        {
            timeCountDownText.text = timeInt.ToString("D");
        }
        else
        {
            timeCountDownText.text = "";
            timeCountDownText.gameObject.SetActive(false); //should disable the timeCountDownText either
        }
    }
}
