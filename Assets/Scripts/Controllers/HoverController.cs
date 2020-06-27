using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    Outline outline;
    [SerializeField] Color outlineColor;
    [SerializeField] float outlineWidth;

    private void Awake() 
    {
        outline = gameObject.AddComponent<Outline>();    
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;
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
