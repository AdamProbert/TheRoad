using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Actions/Overwatch")]
public class OverwatchAction : BaseAction
{
    CharacterEventManager characterEventManager;
    public override void Initialize(GameObject obj)
    {
        characterEventManager = obj.GetComponent<CharacterEventManager>();
    }

    public override void TriggerAction()
    {

    }

    public override void EnableAction()
    {
        characterEventManager.OnCharacterRequestChangeState(CharacterState.OVERWATCHSETUP);
    }

    public override void DisableAction()
    {
        characterEventManager.OnCharacterRequestChangeState(CharacterState.WAITING);
    }
}
