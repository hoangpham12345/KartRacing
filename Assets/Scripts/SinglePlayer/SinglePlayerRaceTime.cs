using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SinglePlayerRaceTime : MonoBehaviour
{
    private TMP_Text time;
    public static double timeElapsed;

    private void Awake()
    {
        time = SinglePlayerGameManager.instance.time;

    }

    private void OnEnable()
    {
        time.gameObject.SetActive(true);
        timeElapsed = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!SinglePlayerLapControler.isFnished)   // if race still continue
        {
            timeElapsed += Time.deltaTime;
            time.text = "Time: " + Math.Round(timeElapsed, 2);
        }
    }
}
