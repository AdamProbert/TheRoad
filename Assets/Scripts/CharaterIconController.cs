using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharaterIconController : MonoBehaviour
{

    [Header("Icons")]
    [SerializeField] GameObject selectionRing;
    [SerializeField] GameObject stealthIcon;
    [SerializeField] GameObject stealthIndicator;

    CharacterEventManager characterEventManager;

    bool isStealthed = false;
    
    private void Awake() 
    {
        characterEventManager = GetComponentInParent<CharacterEventManager>();    
    }

    private void HandleChangeStealthState(CharacterStealthState newState)
    {
        if(newState == CharacterStealthState.NORMAL)
        {
            isStealthed = false;
            selectionRing.SetActive(true);
            stealthIcon.SetActive(false);
        }

        else if(newState == CharacterStealthState.SNEAKING)
        {
            isStealthed = true;
            selectionRing.SetActive(false);
            stealthIcon.SetActive(true);
        }
    }

    private void HandleCharacterVisibilityChange(float newVisibilty)
    {
        Debug.Log("New visibilty: " + newVisibilty);
        // New visibilty = -1 to 1 - turn this into a value between 0 and 1 for the icon change
        float v = Mathf.Clamp(newVisibilty, 0, 1);
        stealthIndicator.transform.DOScale(new Vector3(1 + v, 1 + v, 1 + v), .5f);
    }

    private void HandleCharacterDie()
    {
        selectionRing.SetActive(false);
        stealthIndicator.SetActive(false);
    }

    private void HandleCharacterSelect(bool isSelected)
    {
        if(isSelected)
        {
            if(isStealthed)
            {
                stealthIcon.SetActive(true);
            }
            else
            {
                selectionRing.SetActive(true);
            }
        }
        else
        {
            stealthIcon.SetActive(false);
            selectionRing.SetActive(false);
        }
    }

    private void OnEnable() 
    {                
        characterEventManager.OnCharacterChangeStealthState += HandleChangeStealthState;
        characterEventManager.OnCharacterVisibilityChange += HandleCharacterVisibilityChange;
        characterEventManager.OnCharacterDied += HandleCharacterDie;
        characterEventManager.OnCharacterSelected += HandleCharacterSelect;
    }
    
    private void OnDisable() 
    {        
        characterEventManager.OnCharacterChangeStealthState -= HandleChangeStealthState;
        characterEventManager.OnCharacterVisibilityChange += HandleCharacterVisibilityChange;
        characterEventManager.OnCharacterDied += HandleCharacterDie;
        characterEventManager.OnCharacterSelected -= HandleCharacterSelect;
    }
}
