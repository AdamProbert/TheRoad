using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(CharacterData))]
public class MovementController : MonoBehaviour
{
    [SerializeField] LayerMask walkableLayersForMousePosition;
    [SerializeField] LayerMask coverLayer;
    [SerializeField] float movementLineYPosOffset;
    Camera cam;
    CharacterEventManager characterEventManager;
    CharacterData characterData;
    NavMeshAgent agent;
    NavMeshPath path;
    LineRenderer lineRenderer;

    Vector3 moveTargetPosition;
    Ray hoverRay;
    RaycastHit hoverHit;
    bool moving = false;
    bool enablePathDraw = false;
    bool actionPositioning;

    private void Awake() 
    {
        cam = Camera.main;
        characterEventManager = GetComponent<CharacterEventManager>();
        characterData = GetComponent<CharacterData>();
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start () 
    {
        path = new NavMeshPath();
    }

    private void Update() 
    {
        if(actionPositioning)
        {
            PositionCharacterForAction();
        }
        if(enablePathDraw)
        {
            DrawPath();
        }
        else
        {
            lineRenderer.enabled = false;
        }

        // Let the rest of the charcter know we reached the destination this frame
        if(GetRemainingDistance() < agent.radius && moving)
        {
            characterEventManager.OnCharacterReachedDetination();
            moving = false;
        }
        else
        {
            moving = true;
        }
    }

    private void EnableActionPositioning()
    {
        SetMoveTarget(transform.position);
        actionPositioning = true;
    }

    private void DisableActionPositioning()
    {
        actionPositioning = false;
    }

    private void SetMoveTarget(Vector3 newTarget)
    {
        moveTargetPosition = newTarget;
        agent.destination = moveTargetPosition;
        enablePathDraw = false;
    }

    private void ShowMove()
    {
        enablePathDraw = true;
    }

    private void MoveToMousePosition()
    {
        Vector3 mousePosition = GetMousePositionWithCover();
        if(mousePosition != Vector3.zero)
        {
            moveTargetPosition = mousePosition;
            agent.destination = moveTargetPosition;
        }
        enablePathDraw = false;
    }

    private void DrawPath()
    {
        Vector3 mousePosition = GetMousePositionWithCover();
        if(mousePosition != Vector3.zero)
        {   
            agent.CalculatePath(mousePosition, path);
            Vector3[] corners = path.corners;
            // Offset line render position
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i].y += movementLineYPosOffset;
            }
            lineRenderer.enabled = true;
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void PositionCharacterForAction()
    {
        Vector3 mousePosition = GetMousePositionWithCover();
        Vector3 relativePos = mousePosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = rotation;
    }

    private void SneakMode(bool shouldSneak)
    {
        if(shouldSneak)
        {
            agent.speed = characterData.getSneakSpeed;
        }
        else
        {
            agent.speed = characterData.getMoveSpeed;
        }
    }

    private Vector3 GetMousePositionWithCover()
    {
        hoverRay = cam.ScreenPointToRay(Input.mousePosition);
        // First check for cover
        if(Physics.Raycast(hoverRay, out hoverHit, 9999f, coverLayer, QueryTriggerInteraction.Collide))
        {
            return hoverHit.transform.position;
        }
        // Then non-covered areas
        else if(Physics.Raycast(hoverRay, out hoverHit, 9999f, walkableLayersForMousePosition))
        {
            return hoverHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private float GetPathLength()
    {
        float length = 0.0f;

        for ( int i = 1; i < path.corners.Length; ++i )
        {
            length += Vector3.Distance(path.corners[i-1], path.corners[i]);
        }

        return length;
    }

    float GetRemainingDistance()
    {
        //-- If navMeshAgent is still looking for a path then use line test
         if(agent.pathPending)
         {
            return Vector3.Distance(transform.position, moveTargetPosition);
         }
         else
         {
            return agent.remainingDistance;
         }
    }

    private void HandleStateChange(CharacterState newState)
    {
        if(newState == CharacterState.DEAD)
        {
            this.enabled = false;
        }
        
        if(newState == CharacterState.ATTACKING)
        {
            SetMoveTarget(transform.position);
        }
        else if(
            newState == CharacterState.OVERWATCHSETUP ||
            newState == CharacterState.USINGITEM
        )
        {
            EnableActionPositioning();
        }
        else
        {
            DisableActionPositioning();
        }
    }

    private void HandleStealthStateChange(CharacterStealthState newState)
    {
        if(newState == CharacterStealthState.SNEAKING)
        {
            SneakMode(true);
        }
        else
        {
            SneakMode(false);
        }
    }

    private void HandleCharacterSelected(bool isSelected)
    {
        if(!isSelected)
        {
            DisableActionPositioning();
        }
    }
    private void OnEnable() 
    {
        characterEventManager.OnCharacterChangeState += HandleStateChange;
        characterEventManager.OnCharacterChangeStealthState += HandleStealthStateChange;
        characterEventManager.OnCharacterReceiveNewMovementTarget += SetMoveTarget;
        characterEventManager.OnCharacterMoveRequested += MoveToMousePosition;
        characterEventManager.OnCharacterRequestShowMove += ShowMove;
        characterEventManager.OnCharacterSelected += HandleCharacterSelected;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterChangeState -= HandleStateChange;
        characterEventManager.OnCharacterChangeStealthState -= HandleStealthStateChange;
        characterEventManager.OnCharacterReceiveNewMovementTarget -= SetMoveTarget;
        characterEventManager.OnCharacterMoveRequested -= MoveToMousePosition;
        characterEventManager.OnCharacterRequestShowMove -= ShowMove;
        characterEventManager.OnCharacterSelected -= HandleCharacterSelected;
    }
}
