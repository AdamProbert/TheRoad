using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AttackController))]
[RequireComponent(typeof(CharacterEventManager))]
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

    private void Awake() 
    {
        attackController = GetComponent<AttackController>();
        movementController = GetComponent<MovementController>();
        characterEventManager = GetComponent<CharacterEventManager>();
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
        Debug.Log("Handle character selection called with: " + character);
        if(character == this)
        {
            selectionRing.SetActive(true);
            characterSelected = true;
            this.BroadcastMessage("HandleCharacterSelected", true);
            return;
        }

        if(character != this && characterSelected)
        {
            selectionRing.SetActive(false);
            characterSelected = false;
            this.BroadcastMessage("HandleCharacterSelected", false);
        }
    }
    
    public void Move(Vector3 position)
    {
        if(characterSelected)
        {
            movementController.SetMoveTarget(position);
            ChangeState(CharacterState.MOVING);
        }
    }

    public void SetTarget(Transform newTarget)
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
