using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [Header("Outline effect")]
    [SerializeField] bool enableOutlineEffect;
    [SerializeField] bool alwaysOn;
    [SerializeField] GameObject whatToOutline;
    Outline outline;
    [SerializeField] Outline.Mode outlineMode = Outline.Mode.OutlineAll;
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
            outline = whatToOutline.AddComponent<Outline>();    
            outline.OutlineMode = outlineMode;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = false;
        }

        if(alwaysOn)
        {
            outline.enabled = true;
        }
    }

    public void Hover()
    {
        if(enableOutlineEffect && !alwaysOn)
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
        if(enableOutlineEffect && !alwaysOn)
        {
            outline.enabled = false;
        }

        if(enableRendererEffect)
        {
            rend.enabled = false;
        }
    }
}
