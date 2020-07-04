using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterEventManager))]
public class CharacterActionsManager : MonoBehaviour
{
    [SerializeField] private List<BaseAction> actionPrefabs;
    [SerializeField] private List<BaseAction> actions = new List<BaseAction>();
    [SerializeField] private List<ActionUIButton> actionsImages = new List<ActionUIButton>();
    [SerializeField] private LayoutGroup actionsGroup;
    [SerializeField] private ActionUIButton actionUIPrefab;
    [SerializeField] private Color actionEnableColour = Color.green;

    CharacterEventManager characterEventManager;
    int currentAction = 99;

    private void Awake() 
    {
        characterEventManager = GetComponent<CharacterEventManager>();
    }

    private void Start() 
    {
        InitActionBarUI();    
    }

    private void InitActionBarUI()
    {
        for (int i = 0; i < actionPrefabs.Count; i++)
        {
            BaseAction newAction = Object.Instantiate(actionPrefabs[i]);
            newAction.Initialize(this.gameObject);
            ActionUIButton x = Instantiate(actionUIPrefab, Vector3.zero, Quaternion.identity, actionsGroup.transform);
            x.Initialize(newAction.icon, i);
            actionsImages.Add(x);
            actions.Add(newAction);
        }
        actionsGroup.gameObject.SetActive(false);
    }

    public void AddAction(BaseAction newAction)
    {
        if(!actions.Contains(newAction))
        {
            actions.Add(newAction);    
        }
    }

    public void RemoveAction(BaseAction action)
    {
        if(actions.Contains(action))
        {
            actions.Remove(action);    
        }
    }

    private void CancelCurrentAction()
    {
        // Stop previous actions
        if(currentAction != 99)
        {
            actions[currentAction].DisableAction();
            actionsImages[currentAction].SetImageActive(false);
        }
    }

    private void HandleSelectAction(int actionNumber)
    {
        // Ensure we have that integers actions
        if(actionNumber < actions.Count)
        {
            CancelCurrentAction();

            // Enable new action
            currentAction = actionNumber;
            actionsImages[currentAction].SetImageActive(true);
            BaseAction selected = actions[currentAction];
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

    private void OnEnable() 
    {
            characterEventManager.OnCharacterSelectedAction += HandleSelectAction;
            characterEventManager.OnCharacterSelected += HandleCharacterSelect;
            characterEventManager.OnCharacterMoveRequested += CancelCurrentAction;
    }

    private void OnDisable() 
    {
            characterEventManager.OnCharacterSelectedAction -= HandleSelectAction;
            characterEventManager.OnCharacterSelected -= HandleCharacterSelect;
            characterEventManager.OnCharacterMoveRequested -= CancelCurrentAction;
    }
}
