using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVarsAccess : Singleton<GlobalVarsAccess>
{
    [SerializeField] GlobalVarsScriptableObject variables;

    public float getBuildingSliceTime()
    {
        return variables.buildingSliceTime;
    }
}
