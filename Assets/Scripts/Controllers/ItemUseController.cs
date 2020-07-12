using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LaunchArcRenderer))]
public class ItemUseController : MonoBehaviour
{
    [SerializeField] LayerMask throwableLayers;
    [SerializeField] Transform effectRadiusIcon;
    CharacterEventManager characterEventManager;
    LaunchArcRenderer launchArcRenderer;
    Camera cam;
    Ray throwRay;
    RaycastHit throwHit;
    Item currentItem;

    bool usingItem;
    bool throwingItem;
    private void Awake() 
    {
        characterEventManager = GetComponentInParent<CharacterEventManager>();    
        launchArcRenderer = GetComponent<LaunchArcRenderer>();
        cam = Camera.main;
    }

    private void Update() 
    {
        if(usingItem && !throwingItem)
        {
            Vector3 mousePosition = GetMousePosition();
            if(mousePosition != Vector3.zero)
            {
                launchArcRenderer.RenderArc(transform.position, mousePosition, currentItem.effectRange);
                if(currentItem.effectRadius != 0)
                {
                    EnableEffectRadiusIcon();
                    effectRadiusIcon.position = new Vector3(mousePosition.x, mousePosition.y + 0.25f, mousePosition.z);
                }
            }
            else
            {
                launchArcRenderer.StopRenderArc();
                DisableEffectRadiusIcon();
            }
        }    
    }

    public void HandleUseItem(Item item)
    {
        // If we're mid use, return the new item
        if(usingItem == true || throwingItem == true)
        {
            item.previousSlot.AddItem(currentItem);
            return;
        }

        if(item.itemType == ItemType.CONSUMABLE)
        {
            UseConsubaleItem(item);
        }
        else if(item.itemType == ItemType.THROWABLE)
        {
            UseThrowableItem(item);
        }
        else
        {
            Debug.LogWarning("HandleUseItem: Unknown item type. " + item.itemType);
        }
    }

    private void EnableEffectRadiusIcon()
    {
        effectRadiusIcon.gameObject.SetActive(true);
        effectRadiusIcon.localScale = new Vector3(currentItem.effectRadius * 2.5f, 0, currentItem.effectRadius * 2.5f);
    }

    private void DisableEffectRadiusIcon()
    {
        effectRadiusIcon.gameObject.SetActive(false);
    }

    private void UseConsubaleItem(Item item)
    {
        characterEventManager.OnCharacterHeal(item.effectValue);
        Destroy(item.gameObject);
    }

    private void UseThrowableItem(Item item)
    {
        characterEventManager.OnCharacterRequestChangeState(CharacterState.USINGITEM);
        usingItem = true;
        currentItem = item;
        item.DisableVisuals();
        Debug.Log("We preparing to throw a ting");
    }

    private void ThrowItemToPosition(Vector3 position)
    {
        if(currentItem && usingItem)
        {   
            throwingItem = true;
            Vector3 mousePosition = GetMousePosition();
            float distance = Vector3.Distance(transform.position, mousePosition);
            if(distance < currentItem.effectRange && mousePosition != Vector3.zero)
            {
                currentItem.EnableVisuals();
                currentItem.transform.position = transform.position;
                List<Vector3> arcPositions = launchArcRenderer.GetArcPositions(transform.position, mousePosition);
                launchArcRenderer.StopRenderArc();
                DisableEffectRadiusIcon();
                currentItem.transform.DOPath(arcPositions.ToArray(),  1f).SetEase(Ease.InCirc).OnComplete(OnCompleteThrow);
                Debug.Log("We throwing a ting");
            }
        }
    }

    private void OnCompleteThrow()
    {
        Debug.Log("On complete throw called");
        Debug.Log("CurrentItem: " + currentItem);
        Debug.Log("UsingItem: " + usingItem);
        currentItem.ActiveEffect();
        currentItem.DisableVisuals();
        Destroy(currentItem.gameObject, 5f);
        usingItem = false;
        currentItem = null;
        throwingItem = false;
        Debug.Log("On complete throw finished");
        characterEventManager.OnCharacterRequestChangeState(CharacterState.WAITING);   
    }

    private Vector3 GetMousePosition()
    {
        throwRay = cam.ScreenPointToRay(Input.mousePosition);
        // First check for cover
        if(Physics.Raycast(throwRay, out throwHit, 9999f, throwableLayers))
        {
            Debug.DrawRay(throwRay.origin, throwRay.direction * throwHit.distance, Color.green, 1f);
            return throwHit.point;
        }
        else
        {
            Debug.DrawRay(throwRay.origin, throwRay.direction * 100f, Color.red, 1f);
            return Vector3.zero;
        }
    }

    private void HandleStateChange(CharacterState newState)
    {
        if(newState == CharacterState.DEAD)
        {
            this.gameObject.SetActive(false);
        }

        if(newState != CharacterState.USINGITEM && usingItem)
        {
            CancelItemUse();
        }
    }

    private void CancelItemUse()
    {
        if(usingItem && !throwingItem)
        {
            // Add the item back to the slot
            currentItem.previousSlot.AddItem(currentItem);
            launchArcRenderer.StopRenderArc();
            DisableEffectRadiusIcon();
            currentItem = null;
            usingItem = false;
        }
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterUseItem += HandleUseItem;
        characterEventManager.OnCharacterChangeState += HandleStateChange;
        characterEventManager.OnCharacterRequestPosition += ThrowItemToPosition;
        characterEventManager.OnCharacterCancelAction += CancelItemUse;
    }
    
    private void OnDisable() 
    {
        characterEventManager.OnCharacterUseItem -= HandleUseItem;
        characterEventManager.OnCharacterChangeState -= HandleStateChange;
        characterEventManager.OnCharacterRequestPosition -= ThrowItemToPosition;
        characterEventManager.OnCharacterCancelAction -= CancelItemUse;
    }
}
