using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GlobalVarsScriptableObject", menuName = "ScriptableObjects/Global", order = 1)]
public class GlobalVarsScriptableObject : ScriptableObject 
{
    public float buildingSliceTime;
}