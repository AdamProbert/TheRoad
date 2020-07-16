using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterEventManager : MonoBehaviour
{
    public Action OnCharacterReachedDetination = delegate{};
    public Action<CharacterState> OnCharacterChangeState = delegate{};
    public Action<Entity> OnCharacterReceiveNewAttackTarget = delegate{};
    public Action<Vector3> OnCharacterReceiveNewMovementTarget = delegate{};
    public Action OnCharacterMoveRequested = delegate{};
    public Action OnCharacterRequestShowMove = delegate{};
    public Action<CharacterState> OnCharacterRequestChangeState = delegate{}; // For other scripts to request a change of state
    public Action<int> OnCharacterSelectedAction = delegate{};
    public Action<Item> OnCharacterUseItem = delegate{};
    public Action<int> OnCharacterUseItemByIndex = delegate{};
    public Action OnCharacterCancelAction = delegate{};
    public Action<Vector3> OnCharacterRequestPosition = delegate{};
    public Action<bool> OnCharacterSelected = delegate{}; // True if selected, false if not
    public Action OnCharacterDied = delegate{};
    public Action<Lootbox> OnCharacterSelectedLootbox = delegate{};
    public Action<float> OnCharacterHeal = delegate{};
    public Action<CharacterStealthState> OnCharacterChangeStealthState = delegate{};
    public Action<float> OnCharacterVisibilityChange = delegate{};
}
