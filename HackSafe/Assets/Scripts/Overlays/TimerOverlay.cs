using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerOverlay : DraggableOverlay
{
    [SerializeField] Text label;

    public bool TimerOn = false;

    protected float timeLeft = 0;

    public delegate void TimerFinishCountingHandler ();
    public event TimerFinishCountingHandler OnFinishCounting;

    public void SetTimer (float time)
    {
        timeLeft = time;
        TimerOn = true;
    }

    protected void OnDisable ()
    {
        timeLeft = 0;
        TimerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                updateTimeLabel (timeLeft);
            }
            else
            {
                timeLeft = 0;
                TimerOn = false;

                if (OnFinishCounting != null)
                    OnFinishCounting ();
            }
        }
    }

    protected void updateTimeLabel (float time)
    {
        time += 1;

        float minutes = Mathf.FloorToInt (time / 60);
        float seconds = Mathf.FloorToInt (time % 60);

        label.text = string.Format ("{0:00}:{1:00}", minutes, seconds);
    }
}
