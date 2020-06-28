using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] public enum CoverTypes
{
    HIGH,
    LOW
}

public class Cover : MonoBehaviour
{
    [SerializeField] CoverTypes coverType;
    [SerializeField] Color highCoverColour;
    [SerializeField] Color lowCoverColour;
    [SerializeField] LayerMask characters;
    private Character occupant;
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if(coverType == CoverTypes.HIGH)
        {
            rend.material.SetColor("_BaseColor", highCoverColour);
        }
        if(coverType == CoverTypes.LOW)
        {
            rend.material.SetColor("_BaseColor", lowCoverColour);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(characters == (characters | (1 << other.gameObject.layer)) && occupant == null)
        {
            occupant = other.gameObject.GetComponentInParent<Character>();
            occupant.HandleEnterCover(this);
        }
    
    }
    private void OnTriggerExit(Collider other) 
    {
        if(characters == (characters | (1 << other.gameObject.layer)) && occupant != null)
        {
            occupant.HandleExitCover();
            occupant = null;
        } 
    }
}
