﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AttackController))]
[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    MovementController movementController;
    AttackController attackController;

    [Header("Data")]
    [SerializeField] CharacterScriptableObject characterData;
    public float remainingHealth;

    [Header("Icons")]
    [SerializeField] GameObject selectionRing;

    CharacterEventManager characterEventManager;

    [SerializeField] public bool friendly;

    public enum CharacterState
    {
        WAITING,
        MOVING,
        ATTACKING
    }

    private bool characterSelected;

    [SerializeField] private CharacterState currentState = CharacterState.WAITING;

    // Cover
    Cover currentCover;
    Animator anim;

    private void Awake() 
    {
        attackController = GetComponent<AttackController>();
        movementController = GetComponent<MovementController>();
        characterEventManager = GetComponent<CharacterEventManager>();
        anim = GetComponent<Animator>();
    }

    public void ChangeState(CharacterState state)
    {
        if(state != currentState)
        {
            currentState = state;
            characterEventManager.OnCharacterChangeState(currentState);
        }
    }

    public void HandleCharacterSelection(Character character)
    {
        if(character == this)
        {
            selectionRing.SetActive(true);
            characterSelected = true;
            this.BroadcastMessage("HandleCharacterSelected", true, SendMessageOptions.DontRequireReceiver);
            return;
        }

        if(character != this && characterSelected)
        {
            selectionRing.SetActive(false);
            characterSelected = false;
            this.BroadcastMessage("HandleCharacterSelected", false, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ShowMove()
    {
        movementController.ShowMove();
    }
    
    public void Move(Vector3 position)
    {
        if(characterSelected)
        {
            movementController.ActivateMove();
            ChangeState(CharacterState.MOVING);
        }
    }

    public void SetAttackTarget(Transform newTarget)
    {
        if(characterSelected)
        {
            attackController.SetTarget(newTarget);
            if(newTarget != null)
            {
                ChangeState(CharacterState.ATTACKING);
            }
        }
    }

    public void Pickup()
    {

    }

    void HandleCharacterReachedDestination()
    {
        if(currentState == CharacterState.MOVING)
        {
            ChangeState(CharacterState.WAITING);
        }
    }

    public void HandleEnterCover(Cover cover)
    {
        currentCover = cover;
        characterEventManager.OnCharacterEnteredCover(cover);
        anim.SetBool("Crouch", true);
    }

    public void HandleExitCover()
    {
        currentCover = null;
        anim.SetBool("Crouch", false);
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPlayerSelectCharacter += HandleCharacterSelection;    
        characterEventManager.OnCharacterReachedDetination += HandleCharacterReachedDestination;
    }
    private void OnDisable() 
    {
        if(PlayerEventManager.Instance)
        {
            PlayerEventManager.Instance.OnPlayerSelectCharacter -= HandleCharacterSelection;
        }
        characterEventManager.OnCharacterReachedDetination -= HandleCharacterReachedDestination;
    }
    
}
