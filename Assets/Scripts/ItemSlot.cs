using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image itemImage;
    [SerializeField] Image borderImage;
    Item currentItem = null;
    Color ogColor;

    bool occupied = false;

    private void Awake() 
    {
        ogColor = borderImage.color;    
    }
    
    public bool HasItem()
    {
        if(occupied)
        {
            return true;
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        borderImage.color =  GlobalVarsAccess.Instance.getHighlightColour();
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        borderImage.color = ogColor;    
    }

    public void AddItem(Item item)
    {
        currentItem = item;
        currentItem.gameObject.SetActive(false);
        itemImage.enabled = true;
        itemImage.sprite = item.itemSprite;
        currentItem.previousSlot = this;
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
