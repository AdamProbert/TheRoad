using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LaunchArcRenderer))]
public class ItemUseController : MonoBehaviour
{
    [SerializeField] LayerMask throwableLayers;
    CharacterEventManager characterEventManager;
    LaunchArcRenderer launchArcRenderer;
    Camera cam;
    Ray throwRay;
    RaycastHit throwHit;
    Item currentItem;

    bool usingItem;
    private void Awake() 
    {
        characterEventManager = GetComponentInParent<CharacterEventManager>();    
        launchArcRenderer = GetComponent<LaunchArcRenderer>();
        cam = Camera.main;
    }

    private void Update() 
    {
        if(usingItem)
        {
            Vector3 mousePosition = GetMousePosition();
            if(mousePosition != Vector3.zero)
            {
                launchArcRenderer.RenderArc(transform.position, mousePosition);
            }
            else
            {
                launchArcRenderer.StopRenderArc();
            }
        }    
    }

    public void HandleUseItem(Item item)
    {
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
            Vector3 mousePosition = GetMousePosition();
            if(mousePosition != Vector3.zero)
            {
                currentItem.EnableVisuals();
                currentItem.transform.position = transform.position;
                List<Vector3> arcPositions = launchArcRenderer.GetArcPositions(transform.position, mousePosition);
                currentItem.transform.DOPath(arcPositions.ToArray(),  1f).SetEase(Ease.InCirc);
                launchArcRenderer.StopRenderArc();
                Debug.Log("We throwing a ting");
                characterEventManager.OnCharacterRequestChangeState(CharacterState.WAITING);
            }
        }
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
        if(newState != CharacterState.USINGITEM)
        {
            usingItem = false;
        }
    }

    private void CancelItemUse()
    {
        if(usingItem)
        {
            // Add the item back to the slot
            currentItem.previousSlot.AddItem(currentItem);
            launchArcRenderer.StopRenderArc();
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
