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

    public int getMinLootPerCrate()
    {
        return variables.minLootPerCrate;
    }

    public int getMaxLootPerCrate()
    {
        return variables.maxLootPerCrate;
    }

    public Item getRandomLootItem()
    {
        return variables.availableItems[Random.Range(0, variables.availableItems.Count)];
    }

    public float getMaxPickUpDistance()
    {
        return variables.maxPickupDistance;
    }
}
