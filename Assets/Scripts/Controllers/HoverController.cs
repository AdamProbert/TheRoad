﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    [Header("Outline effect")]
    [SerializeField] bool enableOutlineEffect;
    [SerializeField] bool alwaysOn;
    [SerializeField] List<GameObject> whatToOutline;
    List<Outline> outlines = new List<Outline>();
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
            foreach (GameObject item in whatToOutline)
            {
                Debug.Log("Hover controller setting color to: " + outlineColor);
                Outline outline = item.AddComponent<Outline>();    
                outline.OutlineMode = outlineMode;
                outline.OutlineColor = outlineColor;
                outline.OutlineWidth = outlineWidth;
                outlines.Add(outline);
            }
        }
    }

    private void Start() 
    {
        foreach(Outline outline in outlines)
        {   
            if(enableOutlineEffect && alwaysOn)
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;        
            }
        }    
    }

    public void Hover()
    {
        if(enableOutlineEffect && !alwaysOn)
        {
            foreach (Outline o in outlines)
            {
                o.enabled = true;    
            }
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
            foreach (Outline o in outlines)
            {
                o.enabled = false;    
            }
        }

        if(enableRendererEffect)
        {
            rend.enabled = false;
        }
    }
}
