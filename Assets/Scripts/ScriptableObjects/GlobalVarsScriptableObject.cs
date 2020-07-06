using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GlobalVarsScriptableObject", menuName = "ScriptableObjects/Global", order = 1)]
public class GlobalVarsScriptableObject : ScriptableObject 
{
    public float buildingSliceTime;

    [Header("Loot")]
    public int maxLootPerCrate;
    public int minLootPerCrate;
    public List<Item> availableItems;

    [Header("Characters")]
    public float maxPickupDistance;
}