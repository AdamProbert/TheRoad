using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [Header("Outline effect")]
    [SerializeField] bool enableOutlineEffect;
    Outline outline;
    [SerializeField] Color outlineColor;
    [SerializeField] float outlineWidth;

    [Header("DisableRendererEffect")]
    [SerializeField] bool enableRendererEffect;
    private Renderer rend;


    private void Awake() 
    {
        if(enableRendererEffect)
        {
            rend = GetComponentInChildren<Renderer>();
            rend.enabled = false;
        }
        
        if(enableOutlineEffect)
        {
            outline = gameObject.AddComponent<Outline>();    
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = false;
        }
    }

    public void Hover()
    {
        if(enableOutlineEffect)
        {
            outline.enabled = true;
        }

        if(enableRendererEffect)
        {
            rend.enabled = true;
        }
    }

    public void StopHover()
    {
        if(enableOutlineEffect)
        {
            outline.enabled = false;
        }

        if(enableRendererEffect)
        {
            rend.enabled = false;
        }
    }
}
