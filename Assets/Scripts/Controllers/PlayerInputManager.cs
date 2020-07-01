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
    [SerializeField] float cameraMoveSpeed;
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

    public void HandleCancelAction()
    {
        currentlySelectedCharacter = null;
        PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
    }

    public void HandleCameraMovement(float horizontal, float vertical, float rotate)
    {
        Vector3 targetPosition = new Vector3(cameraTarget.position.x + (horizontal * cameraMoveSpeed), 0f, cameraTarget.position.z + (vertical * cameraMoveSpeed));
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, targetPosition, cameraMoveSpeed);
        // cameraTarget.Translate(targetDirection *cameraMoveSpeed * Time.fixedUnscaledDeltaTime);

        // Update camera rotation manually due to timescale changes
        freeLookCam.m_XAxis.m_InputAxisValue = rotate * cameraRotateSpeed;
    }
}
