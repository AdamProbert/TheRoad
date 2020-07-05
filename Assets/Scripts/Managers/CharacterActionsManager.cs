using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterEventManager))]
public class CharacterActionsManager : MonoBehaviour
{
    [SerializeField] private List<BaseAction> actionPrefabs; // List of every kind of action
    [SerializeField] private List<ActionSlot> actionSlots; // List of action bar slots
    [SerializeField] private LayoutGroup actionsGroup;
    [SerializeField] private ActionUIButton actionUIPrefab;
    [SerializeField] private Color actionEnableColour = Color.green;

    CharacterEventManager characterEventManager;
    int currentAction;
    readonly int NoAction = 99;

    private void Awake() 
    {
        currentAction = NoAction;
        characterEventManager = GetComponent<CharacterEventManager>();
    }

    private void Start() 
    {
        InitActionBarUI();    
    }

    private void InitActionBarUI()
    {
        // Get all dem slots
        foreach (ActionSlot slot in actionsGroup.GetComponentsInChildren<ActionSlot>())
        {
            actionSlots.Add(slot);
        }

        // Populate with known actions
        for (int i = 0; i < actionPrefabs.Count; i++)
        {
            BaseAction newAction = Object.Instantiate(actionPrefabs[i]);
            newAction.Initialize(this.gameObject);
            ActionUIButton x = Instantiate(actionUIPrefab, actionSlots[i].transform.position, actionSlots[i].transform.rotation, actionSlots[i].transform);
            x.Initialize(newAction.icon, i);
            actionSlots[i].actionImage = x;
            actionSlots[i].action = newAction;
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
            actionSlots[currentAction].action.DisableAction();
            actionSlots[currentAction].actionImage.SetImageActive(false);
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
        if(actionNumber < actionSlots.Count)
        {
            CancelCurrentAction();
            // Enable new action
            currentAction = actionNumber;
            actionSlots[currentAction].actionImage.SetImageActive(true);
            BaseAction selected = actionSlots[currentAction].action;
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
