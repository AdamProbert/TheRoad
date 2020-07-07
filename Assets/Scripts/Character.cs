using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    WAITING,
    MOVING,
    ATTACKING,
    OVERWATCHSETUP,
    OVERWATCH,
    DEAD
}

[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterData))]
[RequireComponent(typeof(AudioSource))]
public class Character : Entity
{
    MovementController movementController;
    AttackController attackController;
    CharacterData characterData;
    AudioSource audioSource;
    Animator anim;

    [Header("Icons")]
    [SerializeField] GameObject selectionRing;

    CharacterEventManager characterEventManager;

    private bool characterSelected;
    [SerializeField] private CharacterState currentState = CharacterState.WAITING;

    [Header("UI")]
    CharacterPortrait portrait;


    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        attackController = GetComponent<AttackController>();
        movementController = GetComponent<MovementController>();
        characterEventManager = GetComponent<CharacterEventManager>();
        anim = GetComponent<Animator>();
        characterData = GetComponent<CharacterData>();
        base.alegiance = Alegiance.FRIENDLY;
    }

    private void Start() 
    {
        UIManager.Instance.Register(this, characterData.getPortrait);    
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

    public void UseConsubale(float amount)
    {
        characterData.currentHealth = Mathf.Clamp(characterData.currentHealth + amount, 0, characterData.getMaxHealth);        UIManager.Instance.UpdateHealth(this, characterData.getMaxHealth, characterData.currentHealth);
        UIManager.Instance.UpdateHealth(this, characterData.getMaxHealth, characterData.currentHealth);
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

    public void HandleSelectInteractable(Lootbox box)
    {
        characterEventManager.OnCharacterSelectedLootbox(box);
        characterEventManager.OnCharacterReceiveNewMovementTarget(box.interactionMovePosition.position);
        ChangeState(CharacterState.MOVING);
    }

    public override void TakeHit(Vector3 direction, float damage)
    {
        characterData.currentHealth -= damage;
        UIManager.Instance.UpdateHealth(this, characterData.getMaxHealth, characterData.currentHealth);
        Instantiate(characterData.getHitEffect, base.GetAimPointPosition(), Quaternion.LookRotation(direction));
        if(characterData.currentHealth <= 0)
        {
            Die();
        }
        else
        {
            audioSource.PlayOneShot(characterData.getRandomHitSound, 1);
        }
    }

    private void Die()
    {
        audioSource.PlayOneShot(characterData.getDeathSound, 1f);
        anim.SetTrigger("die");
        alive = false;
        ChangeState(CharacterState.DEAD);
        PlayerEventManager.Instance.OnCharacterDied(this);
    }

    public void HandleActionSelect(int actionNumber)
    {
        if(characterSelected)
        {
            characterEventManager.OnCharacterSelectedAction(actionNumber);
        }
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
        // Handling requests for state changes explicitly.. not the best. Works for now
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
        else if(newState == CharacterState.WAITING && (currentState == CharacterState.OVERWATCHSETUP || currentState == CharacterState.OVERWATCH))
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
