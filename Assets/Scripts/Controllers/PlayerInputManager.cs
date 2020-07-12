﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour
{
    private Character currentlySelectedCharacter;
    [SerializeField] List<Character> availableCharacters;

    [Header("Cameras")]
    [SerializeField] Camera cam;
    [SerializeField] Transform cameraTarget;
    [SerializeField] float cameraBaseMoveSpeed;
    [SerializeField] float cameraRotateSpeed;
    [SerializeField] CinemachineFreeLook freeLookCam;

    [SerializeField] float camSmoothTime;
    Vector3 camVelocity = Vector3.zero;
    Ray hoverRay;
    RaycastHit hoverHit;

    

    public void HandleClickEntity(Entity entity)
    {
        if(entity.alegiance == Alegiance.FRIENDLY && entity.isAlive())
        {
            currentlySelectedCharacter = entity as Character;
            PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
        }
        else
        {
            if(currentlySelectedCharacter != null)
            {
                currentlySelectedCharacter.SetAttackTarget(entity);
            }
        }
    }

    public void HandleClickUI()
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
                    ItemSlot slot = go.gameObject.GetComponent<ItemSlot>();
                    if(slot.HasItem())
                    {
                        Item item = slot.RemoveItem();
                        currentlySelectedCharacter.UseItem(item);
                    }
                }
            }
        }
    }

    public void HandleClickInteractable(Lootbox box)
    {
        if(currentlySelectedCharacter)
        {
            currentlySelectedCharacter.HandleSelectInteractable(box);
        }
    }

    public void HandleCycleSelectedCharacter()
    {
        if(!currentlySelectedCharacter)
        {
            currentlySelectedCharacter = availableCharacters[0];
        }
        else
        {
            int currentIndex = availableCharacters.IndexOf(currentlySelectedCharacter);
            if(currentIndex == availableCharacters.Count -1)
            {
                currentIndex = -1;
            }
            currentlySelectedCharacter = availableCharacters[currentIndex +1];
        }

        PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
    }

    public void HandleRightClickSpace(Vector3 position, bool buttonUp)
    {
        if(currentlySelectedCharacter != null)
        {
            if(buttonUp)
            {
                currentlySelectedCharacter.HandleRightClick(position);
            }
            else if(!buttonUp)
            {
                currentlySelectedCharacter.ShowMove();
            }
            
        }
    }

    private void HandleActionClicked(int actionNumber)
    {
        Debug.Log("Player input manager told to handle action index: " + actionNumber);
        if(currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.HandleActionSelect(actionNumber);
        }
    }

    public void HandleLeftClickPositon(Vector3 position)
    {
        if(currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.HandleLeftClickEmptyPositon(position);
        }
        // else
        // {
        //     PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
        // }
    }

    public void HandleCameraMovement(float horizontal, float vertical, float rotate)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;
        targetDirection = cameraTarget.TransformDirection(targetDirection);
        targetDirection.y = 0.0f;

        float cameraMoveSpeed = cameraBaseMoveSpeed * Vector3.Distance(cameraTarget.position, cam.transform.position);
        cameraTarget.position = Vector3.SmoothDamp(
            cameraTarget.position,
            cameraTarget.position + (targetDirection * cameraMoveSpeed * Time.deltaTime),
            ref camVelocity,
            camSmoothTime
        );
        
        cameraTarget.Rotate(0, rotate * cameraRotateSpeed, 0);
        
        // Update camera rotation manually due to timescale changes
        // freeLookCam.m_XAxis.m_InputAxisValue = rotate * cameraRotateSpeed;
    }

    private void RemoveCharacter(Character character)
    {
        if(availableCharacters.Contains(character))
        {
            availableCharacters.Remove(character);
            if(currentlySelectedCharacter == character)
            {
                currentlySelectedCharacter = null;
            }
        }
    }

    public Vector3 GetCurrentCharacterPosition()
    {
        return currentlySelectedCharacter.transform.position;
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPlayerClickedAction += HandleActionClicked;
        PlayerEventManager.Instance.OnCharacterDied += RemoveCharacter;
    }

    private void OnDisable() 
    {
        if(PlayerEventManager.Instance != null)
        {
            PlayerEventManager.Instance.OnPlayerClickedAction -= HandleActionClicked;
            PlayerEventManager.Instance.OnCharacterDied -= RemoveCharacter;
        }
    }
}
