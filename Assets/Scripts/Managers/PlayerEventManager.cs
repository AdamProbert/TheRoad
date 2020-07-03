using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// An Event manager for handling player inputs
public class PlayerEventManager : Singleton<PlayerEventManager>
{
    public Action<Character> OnPlayerSelectCharacter = delegate{};
    public Action<Character> OnPlayerHoverCharacter = delegate{};
    public Action OnPauseTime = delegate{};
    public Action OnResumeTime = delegate{};
}
