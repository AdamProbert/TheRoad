using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TimeController : Singleton<TimeController>
{
    // Toggles the time scale between 1 and 0.7
    // whenever the user hits the Fire1 button.

    private float fixedDeltaTime;
    private bool isPaused = false;
    [SerializeField] float slowTimeSpeed;
    [SerializeField] float pauseTimeSpeed;

    void Awake()
    {
        // Make a copy of the fixedDeltaTime, it defaults to 0.02f, but it can be changed in the editor
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    public bool IsTimePaused()
    {
        return isPaused;
    }

    public void TogglePause()
    {
        if(isPaused)
        {
            ResumeTime();
            isPaused = false;
        }
        else
        {
            PauseTime();
            isPaused = true;
        }
    }

    public void SlowTime()
    {
        Time.timeScale = slowTimeSpeed;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    public void PauseTime()
    {
        Time.timeScale = pauseTimeSpeed;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}