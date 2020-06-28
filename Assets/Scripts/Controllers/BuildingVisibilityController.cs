using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVisibilityController : MonoBehaviour
{

    [SerializeField] Transform crossSectionBox;
    [SerializeField] float floorCrossSectionYPosition;
    [SerializeField] LayerMask characters;
    float timeToDissolve;

    int noOfOccupants = 0;
    float originalCrossSectionYPosition;
    bool mouseHovering;

    private void Start() 
    {
        originalCrossSectionYPosition = crossSectionBox.localPosition.y;    
        timeToDissolve = GlobalVarsAccess.Instance.getBuildingSliceTime();
    }
    

    void HandleOccupantCountChange()
    {
        if(noOfOccupants > 0)
        {
            SetVisible(false);
        }
        else if(noOfOccupants <= 0 && !mouseHovering)
        {
            SetVisible(true);
        }
    }

    void SetVisible(bool val)
    {
        if(val)
        {
            StartCoroutine(MoveBox(crossSectionBox.localPosition, new Vector3(crossSectionBox.localPosition.x, originalCrossSectionYPosition, crossSectionBox.localPosition.z)));
        }
        else
        {
            StartCoroutine(MoveBox(crossSectionBox.localPosition, new Vector3(crossSectionBox.localPosition.x, floorCrossSectionYPosition, crossSectionBox.localPosition.z)));
        }
    }

    private IEnumerator MoveBox(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / timeToDissolve;
        while(ratio < 1f)
        {
            elapsedTime += Time.fixedUnscaledDeltaTime;
            ratio = elapsedTime / timeToDissolve;
            crossSectionBox.localPosition = Vector3.Lerp(startPos, endPos, ratio);      
            yield return null;
        }
    }

    void OnMouseEnter()
    {
        SetVisible(false);
        mouseHovering = true;
    }

    void OnMouseExit()
    {
        mouseHovering = false;
        HandleOccupantCountChange();
    }

    private void OnTriggerEnter(Collider other) 
    {   
        if(characters == (characters | (1 << other.gameObject.layer)))
        {
            noOfOccupants += 1;
            HandleOccupantCountChange();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(characters == (characters | (1 << other.gameObject.layer)))
        {
            noOfOccupants -= 1;
            HandleOccupantCountChange();
        }
    }
}
