using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    [SerializeField] ItemScriptableObject itemData;
    protected int remainingUses;

    public void Initialize()
    {
        remainingUses = itemData.uses;
    }
    
    public Sprite itemSprite{
        get {return itemData.icon;}
        set {}
    }

    public string itemName{
        get {return itemData.itemName;}
        set {}
    }

    public abstract void UseItem();
}
