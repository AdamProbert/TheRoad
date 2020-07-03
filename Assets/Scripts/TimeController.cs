using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// NOTE:
// THIS IS BUGGY WITH RAGDOLLS.. HAVE REMOVED USAGE FOR NOW.
// If required will need to solve bugs on rb's jumping on time pause

public class TimeController : Singleton<TimeController>
{
    // Toggles the time scale between 1 and 0.7
    // whenever the user hits the Fire1 button.

    private float fixedDeltaTime;
    private float maxDeltaTime;
    private bool isPaused = false;
    [SerializeField] float slowTimeSpeed;
    [SerializeField] float pauseTimeSpeed;

    void Awake()
    {
        // Make a copy of the fixedDeltaTime, it defaults to 0.02f, but it can be changed in the editor
        this.fixedDeltaTime = Time.fixedDeltaTime;
        this.maxDeltaTime = Time.maximumDeltaTime;
        Debug.Log("maxDeltaTime " + maxDeltaTime);
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
        Time.maximumDeltaTime = this.maxDeltaTime * Time.timeScale;
    }

    public void PauseTime()
    {
        Debug.Log("Pausing time");
        Time.timeScale = pauseTimeSpeed;
        Time.fixedDeltaTime = pauseTimeSpeed;
        Time.maximumDeltaTime = pauseTimeSpeed;
        Debug.Log("Time paused");
        Debug.Log("Timescale: " + Time.timeScale + " FixedDelta: " + Time.fixedDeltaTime + " maxDelta: " + Time.maximumDeltaTime);
    }

    public void ResumeTime()
    {
        Debug.Log("Resuming time");
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime;
        Time.maximumDeltaTime = this.maxDeltaTime;
        Debug.Log("Time resumed");
        Debug.Log("Timescale: " + Time.timeScale + " FixedDelta: " + Time.fixedDeltaTime + " maxDelta: " + Time.maximumDeltaTime);
    }
}