using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemImage;
    Item currentItem = null;

    bool occupied = false;
    
    public bool HasItem()
    {
        if(occupied)
        {
            return true;
        }
        return false;
    }
    public void AddItem(Item item)
    {
        currentItem = item;
        currentItem.gameObject.SetActive(false);
        itemImage.enabled = true;
        itemImage.sprite = item.itemSprite;
        occupied = true;
    }

    public Item RemoveItem()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        currentItem.gameObject.SetActive(true);
        occupied = false;
        return currentItem;
    }
}
