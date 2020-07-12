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
    USINGITEM,
    DEAD,
}

[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterData))]
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
            CharacterAudioManager.Instance.PlayRandoSound(characterData.getSelectionSounds, this, CharacterAudioCategory.SELECTION);
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
    public void CancelAction()
    {
        characterEventManager.OnCharacterCancelAction();
        ChangeState(CharacterState.WAITING);
    }

    public void ShowMove()
    {
        if(currentState != CharacterState.USINGITEM)
        {
            characterEventManager.OnCharacterRequestShowMove();
        }
    }
    
    // Usually move - unless performing an action - then cancel.
    public void HandleRightClick(Vector3 position)
    {
        if(characterSelected)
        {
            if(currentState == CharacterState.USINGITEM)
            {
                CancelAction();
                return;
            }
            if(currentState == CharacterState.ATTACKING)
            {
                SetAttackTarget(null);
            }
            

            CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmMoveSounds, this, CharacterAudioCategory.MOVE);
            characterEventManager.OnCharacterMoveRequested();
            ChangeState(CharacterState.MOVING);
        }
    }

    public void UseItem(Item item)
    {
        characterEventManager.OnCharacterUseItem(item);
        CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmActionSounds, this, CharacterAudioCategory.ACTION);
    }

    public void SetAttackTarget(Entity newTarget)
    {
        if(characterSelected)
        {
            characterEventManager.OnCharacterReceiveNewAttackTarget(newTarget);
            if(newTarget != null && newTarget.alegiance == Alegiance.ENEMY)
            {
                ChangeState(CharacterState.ATTACKING);
                CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmActionSounds, this, CharacterAudioCategory.ACTION);
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
        CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmActionSounds, this, CharacterAudioCategory.ACTION);
        ChangeState(CharacterState.MOVING);
    }

    private void HandleCharacterHeal(float amount)
    {
        characterData.currentHealth = Mathf.Clamp(characterData.currentHealth + amount, 0, characterData.getMaxHealth);        UIManager.Instance.UpdateHealth(this, characterData.getMaxHealth, characterData.currentHealth);
        UIManager.Instance.UpdateHealth(this, characterData.getMaxHealth, characterData.currentHealth);
    }

    public override void TakeHit(Vector3 direction, float damage)
    {
        if(alive)
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
                CharacterAudioManager.Instance.PlayRandoSound(characterData.getHitSounds, this, CharacterAudioCategory.HIT, true);
            }
        }
    }

    private void Die()
    {
        CharacterAudioManager.Instance.PlaySound(characterData.getDeathSound, this, CharacterAudioCategory.DEATH);
        anim.SetTrigger("die");
        selectionRing.SetActive(false);
        alive = false;
        ChangeState(CharacterState.DEAD);
        PlayerEventManager.Instance.OnCharacterDied(this);
    }

    public void HandleActionSelect(int actionNumber)
    {
        if(characterSelected)
        {
            CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmActionSounds, this, CharacterAudioCategory.ACTION);
            characterEventManager.OnCharacterSelectedAction(actionNumber);
        }
    }

    public void HandleLeftClickEmptyPositon(Vector3 position)
    {
        if(currentState == CharacterState.OVERWATCHSETUP)
        {
            ChangeState(CharacterState.OVERWATCH);
        }
        if(currentState == CharacterState.USINGITEM)
        {
            characterEventManager.OnCharacterRequestPosition(position);
        }
    }
    
    private void HandleOtherScriptChangingState(CharacterState newState)
    {
        // Handling requests for state changes explicitly.. not the best. Works for now
        if(newState == CharacterState.WAITING &&
            (
                currentState == CharacterState.ATTACKING ||
                currentState == CharacterState.USINGITEM ||
                currentState == CharacterState.OVERWATCH ||
                currentState == CharacterState.OVERWATCHSETUP
            )
        )
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
        else if(newState == CharacterState.USINGITEM)
        {
            ChangeState(newState);
        }
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPlayerSelectCharacter += HandleCharacterSelection;    
        characterEventManager.OnCharacterReachedDetination += HandleCharacterReachedDestination;
        characterEventManager.OnCharacterRequestChangeState += HandleOtherScriptChangingState;
        characterEventManager.OnCharacterHeal += HandleCharacterHeal;
    }

    private void OnDisable() 
    {
        if(PlayerEventManager.Instance)
        {
            PlayerEventManager.Instance.OnPlayerSelectCharacter -= HandleCharacterSelection;
        }
        characterEventManager.OnCharacterReachedDetination -= HandleCharacterReachedDestination;
        characterEventManager.OnCharacterRequestChangeState -= HandleOtherScriptChangingState;
        characterEventManager.OnCharacterHeal -= HandleCharacterHeal;
    }
}
