using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            GetComponent<KartGame.KartSystems.ArcadeKart>().enabled = true;
            playerCamera.gameObject.SetActive(true);
            cinemachineVirtualCamera.gameObject.SetActive(true);
            GetComponent<LapController>().enabled = true;
        }
        else
        {
            GetComponent<KartGame.KartSystems.ArcadeKart>().enabled = false;
            playerCamera.gameObject.SetActive(false);
            cinemachineVirtualCamera.gameObject.SetActive(false);
            GetComponent<LapController>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
