using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterEventManager : MonoBehaviour
{
    public Action OnCharacterReachedDetination = delegate{};
    public Action<int> OnCharacterUsedActions = delegate{};

    public Action<Character.CharacterState> OnCharacterChangeState = delegate{};
    public Action<Cover> OnCharacterEnteredCover = delegate{};
}
