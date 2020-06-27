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
    [SerializeField] CinemachineFreeLook freeLookCam;
    Ray hoverRay;
    RaycastHit hoverHit;


    public void HandleClickCharacter(Character character)
    {
        if(character.friendly)
        {
            currentlySelectedCharacter = character;
            PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
        }
        else
        {
            if(currentlySelectedCharacter != null)
            {
                currentlySelectedCharacter.SetTarget(character.transform);
            }
        }
    }

    public void HandleRightClickSpace(Vector3 position)
    {
        if(currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.SetTarget(null);
            currentlySelectedCharacter.Move(position);
        }
    }

    public void HandleCancelAction()
    {
        currentlySelectedCharacter = null;
        PlayerEventManager.Instance.OnPlayerSelectCharacter(currentlySelectedCharacter);
    }

    public void HandleCameraMovement(float horizontal, float vertical, float rotate)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical).normalized;
        targetDirection = cam.transform.TransformDirection(targetDirection);
        targetDirection.y = 0.0f;
        cameraTarget.Translate(targetDirection *cameraMoveSpeed * Time.fixedUnscaledDeltaTime);

        // Update camera rotation manually due to timescale changes
        freeLookCam.m_XAxis.m_InputAxisValue = rotate;
    }
}
