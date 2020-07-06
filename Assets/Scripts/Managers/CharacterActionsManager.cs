using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterActionsManager : MonoBehaviour
{
    [SerializeField] private List<BaseAction> actionPrefabs; // List of every kind of action
    [SerializeField] private List<BaseAction> availableActions; // List of available actions
    [SerializeField] private List<ActionUIButton> availableActionImages; // List of available actions
    [SerializeField] private LayoutGroup actionsGroup;
    [SerializeField] private ActionUIButton actionUIPrefab;
    [SerializeField] private Color actionEnableColour = Color.green;

    CharacterEventManager characterEventManager;
    int currentAction;
    readonly int NoAction = 99;

    private void Awake() 
    {
        currentAction = NoAction;
        characterEventManager = GetComponentInParent<CharacterEventManager>();
    }

    private void Start() 
    {
        InitActionBarUI();    
    }

    private void InitActionBarUI()
    {
        // Populate with known actions
        for (int i = 0; i < actionPrefabs.Count; i++)
        {
            BaseAction newAction = Object.Instantiate(actionPrefabs[i]);
            newAction.Initialize(transform.parent.gameObject);
            ActionUIButton x = Instantiate(actionUIPrefab, actionsGroup.transform.position, Quaternion.identity, actionsGroup.transform);
            x.Initialize(newAction.icon, i);
            availableActionImages.Add(x);
            availableActions.Add(newAction);
        }
        actionsGroup.gameObject.SetActive(false);
    }

    // public void AddAction(BaseAction newAction)
    // {
    //     if(!actions.Contains(newAction))
    //     {
    //         actions.Add(newAction);    
    //     }
    // }

    // public void RemoveAction(BaseAction action)
    // {
    //     if(actions.Contains(action))
    //     {
    //         actions.Remove(action);    
    //     }
    // }

    private void CancelCurrentAction()
    {
        // Stop previous actions
        if(currentAction != NoAction)
        {
            availableActions[currentAction].DisableAction();
            availableActionImages[currentAction].SetImageActive(false);
            currentAction = NoAction;
        }
    }

    private void HandleSelectAction(int actionNumber)
    {
        if(actionNumber == currentAction)
        {
            CancelCurrentAction();
            return;
        }

        // Ensure we have that integers actions
        if(actionNumber < availableActions.Count)
        {
            CancelCurrentAction();
            // Enable new action
            currentAction = actionNumber;
            availableActionImages[currentAction].SetImageActive(true);
            BaseAction selected = availableActions[currentAction];
            if(selected.actionType == ACTIONTYPE.TRIGGER)
            {
                selected.TriggerAction();
            }
            else
            {
                selected.EnableAction();
            }
        }
    }

    private void HandleCharacterSelect(bool isISelected)
    {
        actionsGroup.gameObject.SetActive(isISelected);
    }

    private void HandleStateChange(CharacterState newState)
    {
        if(newState == CharacterState.DEAD)
        {
            CancelCurrentAction();
            this.enabled = false;
        }
    }

    private void OnEnable() 
    {
            characterEventManager.OnCharacterSelectedAction += HandleSelectAction;
            characterEventManager.OnCharacterSelected += HandleCharacterSelect;
            characterEventManager.OnCharacterMoveRequested += CancelCurrentAction;
            characterEventManager.OnCharacterChangeState += HandleStateChange;
    }

    private void OnDisable() 
    {
            characterEventManager.OnCharacterSelectedAction -= HandleSelectAction;
            characterEventManager.OnCharacterSelected -= HandleCharacterSelect;
            characterEventManager.OnCharacterMoveRequested -= CancelCurrentAction;
            characterEventManager.OnCharacterChangeState -= HandleStateChange;
    }
}
