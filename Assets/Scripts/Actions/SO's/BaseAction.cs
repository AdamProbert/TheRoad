using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ACTIONTYPE
{
    TRIGGER,
    LOOPING
}
public abstract class BaseAction : ScriptableObject
{
    public string actionName = "New Action";
    public ACTIONTYPE actionType;
    public Sprite icon;
    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAction();
    public abstract void EnableAction();
    public abstract void DisableAction();
}
