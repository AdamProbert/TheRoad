﻿using System.Collections;
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
}
