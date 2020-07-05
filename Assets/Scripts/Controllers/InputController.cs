using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Update() 
    {
        CheckHover();
        CheckLeftMouseButton();
        CheckRightMouseButton();
        CheckCameraMovements();
        CheckActions();
        CheckCharacterSwitch();
    }

    void CheckHover()
    {
        hoverRay = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(hoverRay, out hoverHit, 200f, hoverable))
        {
            if(currentHoveredObject == null || hoverHit.transform != currentHoveredObject)
            {
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
            if (EventSystem.current.IsPointerOverGameObject()){return;} // Ignore ui
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
                    print("Left clicked at position: " + clickPosition);
                    if(clickPosition != Vector3.zero)
                    {
                        playerInputManager.HandleLeftClickPositon(clickPosition);
                    }
                }
            }
        }    
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
        float vertical = Input.GetAxisRaw("Vertical") * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        float rotate = Input.GetAxisRaw("RotateCamera") * Time.deltaTime;
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
