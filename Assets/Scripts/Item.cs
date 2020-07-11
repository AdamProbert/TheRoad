using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    CONSUMABLE,
    THROWABLE
}

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected ItemScriptableObject itemData;
    [SerializeField] public ItemType itemType;
    protected int remainingUses;

    public ItemSlot previousSlot;

    public void Initialize()
    {
        remainingUses = itemData.uses;
    }

    public void DisableVisuals()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;            
        }

        GetComponent<HoverController>().enabled = false;
        GetComponentInChildren<Collider>().enabled = false;
    }

    public void EnableVisuals()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;            
        }

        GetComponent<HoverController>().enabled = true;
        GetComponentInChildren<Collider>().enabled = true;
    }
    
    public Sprite itemSprite{
        get {return itemData.icon;}
        set {}
    }

    public string itemName{
        get {return itemData.itemName;}
        set {}
    }

    public float effectValue{
        get {return itemData.effectValue;}
        set {}
    }

    public abstract void UseItem(Character character);
}
