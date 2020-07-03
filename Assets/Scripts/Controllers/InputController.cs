using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] MovementController movementController;
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] LayerMask clickable; // Should be everything that can be clicked
    [SerializeField] LayerMask entities;
    [SerializeField] Camera cam;

    [Header("Hovering")]
    Ray hoverRay;
    RaycastHit hoverHit;
    HoverController currentHoveredObject;
    [SerializeField] LayerMask hoverable;

    private void Update() 
    {
        CheckHover();
        CheckLeftMouseButton();
        CheckRightMouseButton();
        CheckCameraMovements();
        CheckActions();
        CheckPauseTime();
    }

    void CheckPauseTime()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TimeController.Instance.TogglePause();
        }
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
        // Overwatch
        if(Input.GetKeyDown(KeyCode.O))
        {
            playerInputManager.HandleOverwatchClicked();
        }
    }

    void CheckLeftMouseButton()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Transform clickedObject = GetClickedObject(entities);
            if (clickedObject)
            {
                playerInputManager.HandleClickCharacter(clickedObject.GetComponentInParent<Entity>());
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

    void CheckRightMouseButton()
    {
        if(Input.GetMouseButtonUp(1))
        {
            Vector3 clickPosition = GetClickPosition(clickable);
            playerInputManager.HandleRightClickSpace(clickPosition, true);
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 clickPosition = GetClickPosition(clickable);
            playerInputManager.HandleRightClickSpace(clickPosition, false);
        }
    }

    void CheckCameraMovements()
    {
        float vertical = Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime;
        float rotate = Input.GetAxisRaw("RotateCamera") * Time.unscaledDeltaTime;
        playerInputManager.HandleCameraMovement(horizontal, vertical, rotate);
    }

    Vector3 GetClickPosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
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
