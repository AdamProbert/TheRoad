using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    WAITING,
    MOVING,
    ATTACKING,
    OVERWATCHSETUP,
    OVERWATCH
}

[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterData))]

public class Character : Entity
{
    MovementController movementController;
    AttackController attackController;
    CharacterData characterData;
    Animator anim;

    [Header("Icons")]
    [SerializeField] GameObject selectionRing;

    CharacterEventManager characterEventManager;

    private bool characterSelected;
    [SerializeField] private CharacterState currentState = CharacterState.WAITING;


    private void Awake() 
    {
        attackController = GetComponent<AttackController>();
        movementController = GetComponent<MovementController>();
        characterEventManager = GetComponent<CharacterEventManager>();
        anim = GetComponent<Animator>();
        characterData = GetComponent<CharacterData>();
        base.alegiance = Alegiance.FRIENDLY;
    }

    public void ChangeState(CharacterState state)
    {
        if(state != currentState)
        {
            Debug.Log("New state: " + state);
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
        characterEventManager.OnCharacterRequestShowMove();
    }
    
    public void Move(Vector3 position)
    {
        if(characterSelected)
        {
            characterEventManager.OnCharacterMoveRequested();            
            ChangeState(CharacterState.MOVING);
        }
    }

    public void SetAttackTarget(Entity newTarget)
    {
        if(characterSelected)
        {
            characterEventManager.OnCharacterReceiveNewAttackTarget(newTarget);
            if(newTarget != null)
            {
                ChangeState(CharacterState.ATTACKING);
            }
        }
    }

    void HandleCharacterReachedDestination()
    {
        if(currentState == CharacterState.MOVING)
        {
            ChangeState(CharacterState.WAITING);
        }
    }

    public override void TakeHit(Vector3 direction, float damage)
    {
        Instantiate(characterData.hitEffect, base.GetAimPointPosition(), Quaternion.LookRotation(direction));
        characterData.currentHealth -= damage;
        if(characterData.currentHealth <= 0)
        {
            Debug.Log("Player died");
        }
    }

    public void ToggleOverwatch()
    {
        if(currentState == CharacterState.OVERWATCHSETUP)
        {
            ChangeState(CharacterState.WAITING);
        }
        else
        {
            ChangeState(CharacterState.OVERWATCHSETUP);
        }
    }

    public void HandleLeftClickEmptyPositon(Vector3 position)
    {
        if(currentState == CharacterState.OVERWATCHSETUP)
        {
            ChangeState(CharacterState.OVERWATCH);
        }
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
