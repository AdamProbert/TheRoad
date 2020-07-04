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
            characterEventManager.OnCharacterSelected(true);
            return;
        }

        if(character != this && characterSelected)
        {
            selectionRing.SetActive(false);
            characterSelected = false;
            characterEventManager.OnCharacterSelected(false);
            if(currentState == CharacterState.OVERWATCHSETUP)
            {
                ChangeState(CharacterState.WAITING);
            }
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
        characterData.currentHealth -= damage;
        if(characterData.currentHealth <= 0)
        {
            Debug.Log("Player died");
        }
    }

    public void HandleActionSelect(int actionNumber)
    {
        if(characterSelected)
        {
            characterEventManager.OnCharacterSelectedAction(actionNumber);
        }

        // if(currentState == CharacterState.OVERWATCHSETUP)
        // {
        //     ChangeState(CharacterState.WAITING);
        // }
        // else if(currentState != CharacterState.ATTACKING)s
        // {
        //     ChangeState(CharacterState.OVERWATCHSETUP);
        // }
    }

    public void HandleLeftClickEmptyPositon(Vector3 position)
    {
        if(currentState == CharacterState.OVERWATCHSETUP)
        {
            ChangeState(CharacterState.OVERWATCH);
        }
    }
    
    private void HandleOtherScriptChangingState(CharacterState newState)
    {
        if(newState == CharacterState.WAITING && currentState == CharacterState.ATTACKING)
        {
            ChangeState(newState);
        }
        else if(
            newState == CharacterState.ATTACKING &&
            (
                currentState == CharacterState.OVERWATCH ||
                currentState == CharacterState.OVERWATCHSETUP
            )
        )
        {
            ChangeState(newState);
        }
        else if(newState == CharacterState.OVERWATCHSETUP)
        {
            ChangeState(newState);
        }
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPlayerSelectCharacter += HandleCharacterSelection;    
        characterEventManager.OnCharacterReachedDetination += HandleCharacterReachedDestination;
        characterEventManager.OnCharacterRequestChangeState += HandleOtherScriptChangingState;
    }
    private void OnDisable() 
    {
        if(PlayerEventManager.Instance)
        {
            PlayerEventManager.Instance.OnPlayerSelectCharacter -= HandleCharacterSelection;
        }
        characterEventManager.OnCharacterReachedDetination -= HandleCharacterReachedDestination;
        characterEventManager.OnCharacterRequestChangeState -= HandleOtherScriptChangingState;
    }
    
}
