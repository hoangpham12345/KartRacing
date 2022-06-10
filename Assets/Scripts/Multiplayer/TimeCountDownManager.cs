using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class TimeCountDownManager : MonoBehaviourPun
{
    private TMP_Text timeCountDownText;

    private float timeToStartRace = 3.0f;
    private int oldTimeInt = 3;

    private void Awake()
    {
        // Disbale car movement of the car that attached this only
        timeCountDownText = MultiplayerRacingModeManager.instance.timeCountDownText;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) // only one player should do this, the best one is master client
        {
            if (timeToStartRace >= 0.0f)
            {
                timeToStartRace -= Time.deltaTime;

                int timeInt = (int)Math.Ceiling(timeToStartRace);

                // Raise an RPC call to synchronize the cooldown time for all players
                if (oldTimeInt != timeInt)
                {
                    photonView.RPC("SetTime", RpcTarget.AllBuffered, timeInt);
                    oldTimeInt = timeInt;
                }
            }
            else if (timeToStartRace < 0.0f)
            {
                // When the time stop, it should call this for all player to enable their control
                photonView.RPC("StartTheRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(int timeInt)
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

    [PunRPC]
    public void StartTheRace()
    {
        GetComponent<KartGame.KartSystems.ArcadeKart>().SetCanMove(true);
        this.enabled = false;
    }
}
