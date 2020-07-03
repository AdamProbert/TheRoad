using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    Ray hoverRay;
    RaycastHit hoverHit;

    public void HandleClickCharacter(Entity entity)
    {
        if(entity.alegiance == Alegiance.FRIENDLY)
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

    public void HandleRightClickSpace(Vector3 position, bool buttonUp)
    {
        if(currentlySelectedCharacter != null)
        {
            if(buttonUp)
            {
                currentlySelectedCharacter.SetAttackTarget(null);
                currentlySelectedCharacter.Move(position);
            }
            else if(!buttonUp)
            {
                currentlySelectedCharacter.ShowMove();
            }
            
        }
    }

    public void HandleOverwatchClicked()
    {
        if(currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.ToggleOverwatch();
        }
    }

    public void HandleLeftClickPositon(Vector3 position)
    {
        if(currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.HandleLeftClickEmptyPositon(position);
        }
        else
        {
            PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
        }
            
    }

    public void HandleCameraMovement(float horizontal, float vertical, float rotate)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;
        targetDirection = cameraTarget.TransformDirection(targetDirection);
        targetDirection.y = 0.0f;
        
        cameraTarget.position = Vector3.Lerp(
            cameraTarget.position,
            cameraTarget.position + targetDirection,
            cameraBaseMoveSpeed * Time.unscaledDeltaTime
        );
        // cameraTarget.position += targetDirection * cameraMoveSpeed * Time.unscaledDeltaTime;
        cameraTarget.Rotate(0, rotate * cameraRotateSpeed * Time.unscaledDeltaTime, 0);
        
        // Update camera rotation manually due to timescale changes
        // freeLookCam.m_XAxis.m_InputAxisValue = rotate * cameraRotateSpeed;
    }
}
