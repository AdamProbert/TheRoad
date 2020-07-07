using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
public class HoverController : MonoBehaviour
{
    // [Header("Outline effect")]
    Outline outline;

    private void Awake() 
    {
        outline = GetComponentInChildren<Outline>();
    }

    private void Start() 
    {
        outline.enabled = false;
        Debug.Log("Setting outline to false");
    }

    public void Hover()
    {
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }
}
