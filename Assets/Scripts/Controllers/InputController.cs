using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] LayerMask clickable; // Should be everything that can be clicked
    [SerializeField] LayerMask entities;
    [SerializeField] LayerMask interactables;
    [SerializeField] Camera cam;
    
    [Header("Hovering")]
    [SerializeField] LayerMask hoverable;
    Ray hoverRay;
    RaycastHit hoverHit;
    HoverController currentHoveredObject;

    [Header("Dragging")]
    [SerializeField] LayerMask draggable;
    [SerializeField] LayerMask droppable; // Used for anything the player can drop an item on to
    Ray dragRay;
    RaycastHit dragHit;
    Item draggingItem;
    ItemSlot possibleDragging;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition = Vector3.zero;
    float dragItemZOffset;

    [Header("Camera bounds")]
    [SerializeField][Tooltip("Used for mouse panning at edge of screen")] int screenBoundary;
    [SerializeField] bool enableEdgeScroll;
    int screenWidth;
    int screenHeight;


    KeyCode[] actionKeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,       
    };

    private void Start() 
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        Cursor.lockState = CursorLockMode.Confined;
    } 

    private void Update() 
    {
        CheckHover();
        CheckLeftMouseButton();
        CheckRightMouseButton();
        CheckCameraMovements();
        CheckActions();
        CheckDrag();
        CheckCharacterSwitch();
    }

    void CheckDrag()
    {
        dragRay = cam.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0))
        {    
            // Check for in-game object
            if(Physics.Raycast(dragRay, out dragHit, 200f, draggable))
            {
                Vector3 characterPos = playerInputManager.GetCurrentCharacterPosition();

                if(Vector3.Distance(characterPos, dragHit.point) <= GlobalVarsAccess.Instance.getMaxPickUpDistance())
                {
                    draggingItem = dragHit.transform.GetComponentInParent<Item>();
                    dragItemZOffset = dragHit.distance / 2;
                    originalPosition = draggingItem.transform.position;
                    screenPoint = cam.WorldToScreenPoint(draggingItem.transform.position);
                    offset = draggingItem.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
            }

            // Check for UI object
            if(EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if(raycastResults.Count > 0)
                {
                    foreach(RaycastResult go in raycastResults)
                    {  
                        if(go.gameObject.GetComponent<ItemSlot>())
                        {   
                            possibleDragging = go.gameObject.GetComponent<ItemSlot>();
                            if(!possibleDragging.HasItem())
                            {
                                possibleDragging = null;
                            }
                        }
                    }
                }
            }
        }

        // Check drag from ui to world - need to ensure player is dragging and not clicking
        if(possibleDragging && Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if(possibleDragging.HasItem())
            {
                draggingItem = possibleDragging.RemoveItem();
                originalPosition = Vector3.zero;
                possibleDragging = null;
            }
        }

        if(draggingItem)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragItemZOffset);
            draggingItem.transform.position = cam.ScreenToWorldPoint(curScreenPoint) + offset;
        }

        if(Input.GetMouseButtonUp(0) && draggingItem != null)
        {
            // Check dropping on to hotbar
            if(EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if(raycastResults.Count > 0)
                {
                    foreach(RaycastResult go in raycastResults)
                    {  
                        if(go.gameObject.GetComponent<ItemSlot>())
                        {   
                            go.gameObject.GetComponent<ItemSlot>().AddItem(draggingItem);
                            draggingItem = null;
                        }
                    }
                }
            }
            // Check dropping into world
            else
            {
                // if from world - go back to og position
                if(originalPosition != Vector3.zero)
                {
                    draggingItem.transform.position = originalPosition;
                }
                else
                {
                    dragRay = cam.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(dragRay, out dragHit, 999f, droppable))
                    {
                        Vector3 characterPos = playerInputManager.GetCurrentCharacterPosition();
                        Debug.DrawRay(dragRay.origin, dragRay.direction, Color.white, 5f);

                        if(Vector3.Distance(characterPos, dragHit.point) > GlobalVarsAccess.Instance.getMaxPickUpDistance())
                        {
                            Vector3 dirToMouse = (new Vector3(dragHit.point.x, characterPos.y, dragHit.point.z) - characterPos).normalized;
                            Vector3 dropPoint = characterPos + (dirToMouse * GlobalVarsAccess.Instance.getMaxPickUpDistance());
                            draggingItem.transform.position = dropPoint;
                        }
                        else
                        {
                            draggingItem.transform.position = dragHit.point;
                        }
                    }
                }
                
                draggingItem = null;
            }
        }
    }

    void CheckHover()
    {
        hoverRay = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(hoverRay, out hoverHit, 200f, hoverable))
        {
            Debug.Log("Hovering over: " + hoverHit.transform.name);
            if(currentHoveredObject == null || hoverHit.transform != currentHoveredObject)
            {
                if(currentHoveredObject)
                {
                    currentHoveredObject.StopHover();
                }
                currentHoveredObject = hoverHit.transform.GetComponentInParent<HoverController>();
                currentHoveredObject.Hover();
            }   
        }
        else
        {
            if(currentHoveredObject)
            {
                // No longer hovering
                currentHoveredObject.StopHover();
                currentHoveredObject = null;
            }
        }
    }
    
    void CheckActions()
    {
        for (int i = 0; i < actionKeys.Length; i++)
        {
            if(Input.GetKeyDown(actionKeys[i]))
            {
                PlayerEventManager.Instance.OnPlayerClickedAction(i);
            }
        }
    }

    void CheckLeftMouseButton()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Transform clickedObject = GetClickedObject(clickable);
            if (clickedObject)
            {
                // Check for entity clicks
                if(entities == (entities | (1 << clickedObject.gameObject.layer)))
                {
                    playerInputManager.HandleClickEntity(clickedObject.GetComponentInParent<Entity>());
                }
                else if(interactables == (interactables | (1 << clickedObject.gameObject.layer)))
                {
                    playerInputManager.HandleClickInteractable(clickedObject.GetComponentInParent<Lootbox>());
                }
                else
                {
                    Vector3 clickPosition = GetClickPosition();
                    if(clickPosition != Vector3.zero)
                    {
                        playerInputManager.HandleLeftClickPositon(clickPosition);
                    }
                }
            }
        }
        if(Input.GetMouseButtonUp(0) && !draggingItem && EventSystem.current.IsPointerOverGameObject())
        {
            playerInputManager.HandleClickUI();
            possibleDragging = null;
            draggingItem = null;
        }
    }

    private void OnMouseUpAsButton() 
    {
        
    }

    void CheckRightMouseButton()
    {
        
        if(Input.GetMouseButtonUp(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()){return;} // Ignore ui
            Vector3 clickPosition = GetClickPosition(clickable);
            playerInputManager.HandleRightClickSpace(clickPosition, true);
        }
        if(Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()){return;} // Ignore ui
            Vector3 clickPosition = GetClickPosition(clickable);
            playerInputManager.HandleRightClickSpace(clickPosition, false);
        }
    }

    void CheckCameraMovements()
    {
        // Check physical inputs first
        float vertical = Input.GetAxisRaw("Vertical") * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        float rotate = Input.GetAxisRaw("RotateCamera") * Time.deltaTime;
        
        // Then mouse panning
        if(enableEdgeScroll && vertical == 0 && horizontal == 0)
        {
            if(Input.mousePosition.x > screenWidth - screenBoundary)
            {
                horizontal = 1f;
            }
            else if(Input.mousePosition.x < 0 + screenBoundary)
            {
                horizontal = -1f;
            }
            if(Input.mousePosition.y > screenHeight - screenBoundary)
            {
                vertical = 1f;
            }
            else if(Input.mousePosition.y < 0 + screenBoundary)
            {
                vertical = -1f;
            }
        }

        playerInputManager.HandleCameraMovement(horizontal, vertical, rotate);
    }

    void CheckCharacterSwitch()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            playerInputManager.HandleCycleSelectedCharacter();
        }
    }

    Vector3 GetClickPosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 999f))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 GetClickPosition(LayerMask layerMask)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 999f, layerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Transform GetClickedObject()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform;
        }
        else
        {
            return null;
        }
    }
    Transform GetClickedObject(LayerMask layermask)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 999f, layermask))
        {
            return hit.transform;
        }
        else
        {
            return null;
        }
    }
}
