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
    DEAD
}

public enum CharacterStealthState
{
    SNEAKING,
    NORMAL
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
    [SerializeField] GameObject StealthIcon;
    [SerializeField] GameObject StealthIndicator;

    CharacterEventManager characterEventManager;

    private bool characterSelected;
    [SerializeField] private CharacterState currentState = CharacterState.WAITING;
    [SerializeField] private CharacterStealthState currentStealthState = CharacterStealthState.NORMAL;

    [Header("UI")]
    CharacterPortrait portrait;

    [Header("Visibility")]
    [SerializeField] LayerMask lightBlockingLayers;
    [SerializeField] Light sunlight;
    Light[] lightSources; // Used to determine if in shadow
    float currentVisibility = 0;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        attackController = GetComponent<AttackController>();
        movementController = GetComponent<MovementController>();
        characterEventManager = GetComponent<CharacterEventManager>();
        anim = GetComponent<Animator>();
        characterData = GetComponent<CharacterData>();
        base.alegiance = Alegiance.FRIENDLY;
        lightSources = GameObject.FindObjectsOfType<Light>();
    }

    private void Start() 
    {
        UIManager.Instance.Register(this, characterData.getPortrait);    
    }

    private void Update() 
    {
        CalculateCurrentVisibility();
        if(currentStealthState == CharacterStealthState.SNEAKING)
        {
            UpdateStealthIcon();
        }    
    }

    public bool SettingUpAction()
    {
        if(currentState == CharacterState.USINGITEM || currentState == CharacterState.OVERWATCHSETUP)
        {
            return true;
        }
        return false;
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
        if(SettingUpAction())
        {
            return;
        }

        if(character == this)
        {
            characterSelected = true;
            characterEventManager.OnCharacterSelected(true);
            CharacterAudioManager.Instance.PlayRandoSound(characterData.getSelectionSounds, this, CharacterAudioCategory.SELECTION);
            if(currentStealthState == CharacterStealthState.SNEAKING)
            {
                StealthIcon.SetActive(true);
                selectionRing.SetActive(false);
            }
            else if(currentStealthState == CharacterStealthState.NORMAL)
            {
                selectionRing.SetActive(true);
                StealthIcon.SetActive(false);
            }
            return;
        }

        if(character != this && characterSelected)
        {
            selectionRing.SetActive(false);
            StealthIcon.SetActive(false);
            characterSelected = false;
            characterEventManager.OnCharacterSelected(false);
            if(currentState == CharacterState.OVERWATCHSETUP || currentState == CharacterState.USINGITEM)
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
        if(!SettingUpAction())
        {
            characterEventManager.OnCharacterSelectedLootbox(box);
            characterEventManager.OnCharacterReceiveNewMovementTarget(box.interactionMovePosition.position);
            CharacterAudioManager.Instance.PlayRandoSound(characterData.getConfirmActionSounds, this, CharacterAudioCategory.ACTION);
            ChangeState(CharacterState.MOVING);
        }
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

    public void HandleClickOverwatch()
    {
        if(currentState != CharacterState.OVERWATCHSETUP)
        {
            ChangeState(CharacterState.OVERWATCHSETUP);
        }
        else
        {
            ChangeState(CharacterState.WAITING);
        }
        
    }

    public void HandleClickSneak()
    {
        if(currentStealthState == CharacterStealthState.SNEAKING)
        {
            currentStealthState = CharacterStealthState.NORMAL;
            characterEventManager.OnCharacterChangeStealthState(CharacterStealthState.NORMAL);
            StealthIcon.SetActive(false);
            selectionRing.SetActive(true);
        }
        else if(currentStealthState == CharacterStealthState.NORMAL)
        {
            currentStealthState = CharacterStealthState.SNEAKING;
            characterEventManager.OnCharacterChangeStealthState(CharacterStealthState.SNEAKING);
            StealthIcon.SetActive(true);
            selectionRing.SetActive(false);
        }
    }

    public void HandleSelectItem(int itemIndex)
    {
        characterEventManager.OnCharacterUseItemByIndex(itemIndex);
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
    
    public CharacterState GetCurrentState()
    {
        return currentState;
    }

    private void UpdateStealthIcon()
    {
        // Current visibility ranges from 0 - 30
        // Need to convert this to a variable between 0 & 1 - percentage right?
        float percentVisible = currentVisibility / 30;
        StealthIndicator.transform.localScale = new Vector3(1 + percentVisible, 1 + percentVisible, 1 + percentVisible);
    }

    public void CalculateCurrentVisibility()
    {

        RaycastHit hit;
        float visibility = 20f;

        if(currentStealthState == CharacterStealthState.SNEAKING)
        {
            visibility -= 10f;
        }
        
        // Factor in shadow
        // First sun light
        if(Physics.Linecast(transform.position, sunlight.transform.position, out hit, lightBlockingLayers))
        {
            if(hit.collider.GetComponent<Light>())
            {
                Debug.DrawLine(transform.position, hit.point, Color.white, .1f);
                visibility += 10f;
            }
            else
            {
                visibility -= 10f;
                Debug.DrawLine(transform.position, hit.point, Color.red, .1f);
            }
        }
        else
        {
            Debug.DrawLine(transform.position, sunlight.transform.position, Color.green, .1f);
        }

        currentVisibility = visibility;

        // // Then any other light sources
        // foreach (Light light in lightSources)
        // {
        //     if(Physics.Linecast(transform.position, light.transform.position, out hit, lightBlockingLayers))
        //     {
        //         if(hit.collider.GetComponent<Light>())
        //         {
        //             // Check range to light
        //             if(Vector3.Distance(transform.position, light.transform.position) < light.range)
        //             {
        //                 // Check light pointing in this direction
        //                 if(light.type == LightType.Spot)
        //                 {
        //                     Vector3 dirFromAtoB = (transform.position - light.transform.position).normalized;
        //                     float dotProd = Vector3.Dot(dirFromAtoB, light.transform.forward);
        //                     if(dotProd > 0.9) 
        //                     {
        //                         visibility += 0.2f;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     visibility += 0.2f;
        //                 }
        //             }
        //         }
        //     }
        //}
        // return visibility;
    //}
    
    }

    public float GetCurrentVisibilty()
    {
        return currentVisibility;
    }

    public CharacterStealthState GetCurrentStealthState()
    {
        return currentStealthState;
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
