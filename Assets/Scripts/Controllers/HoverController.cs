using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class HoverController : MonoBehaviour
{
    // [Header("Outline effect")]
    [SerializeField] bool showTooltip;
    [SerializeField] string tooltipText;
    Outline outline;

    private void Start() 
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
    }

    public void Hover()
    {
        if(outline)
        {
            outline.enabled = true;
        }
        if(showTooltip)
        {
            UIManager.Instance.ShowTooltip(tooltipText);
        }
    }

    public void StopHover()
    {
        if(outline)
        {
            outline.enabled = false;
        }
        if(showTooltip && UIManager.Instance)
        {
            UIManager.Instance.HideTooltip();
        }
    }

    private void OnDisable() 
    {
        StopHover();    
    }
}
