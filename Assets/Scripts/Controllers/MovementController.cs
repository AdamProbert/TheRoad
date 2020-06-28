using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CharacterEventManager))]
public class MovementController : MonoBehaviour
{
    [SerializeField] LayerMask walkableLayersForMousePosition;
    private Vector3 moveTargetPosition;
    NavMeshAgent agent;
    NavMeshPath path;
    Ray hoverRay;
    RaycastHit hoverHit;
    [SerializeField] Camera cam;
    LineRenderer lineRenderer;
    CharacterEventManager characterEventManager;

    bool moving = false;

    private bool enablePathDraw = false;

    private void Awake() 
    {
        characterEventManager = GetComponent<CharacterEventManager>();
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start () 
    {
        path = new NavMeshPath();
    }

    public void HandleCharacterSelected(bool isSelected)
    {
        enablePathDraw = isSelected;
    }

    private void Update() 
    {
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

    public void SetMoveTarget(Vector3 newTarget)
    {
        moveTargetPosition = newTarget;
        agent.destination = moveTargetPosition;
    }

    private void DrawPath()
    {
        Vector3 mousePosition = GetMousePosition();
        if(mousePosition != Vector3.zero)
        {   
            agent.CalculatePath(mousePosition, path);
            Vector3[] corners = path.corners;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private Vector3 GetMousePosition()
    {
        hoverRay = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(hoverRay, out hoverHit, 9999f, walkableLayersForMousePosition))
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

    private void HandleStateChange(Character.CharacterState newState)
    {
        if(newState != Character.CharacterState.MOVING)
        {
            SetMoveTarget(transform.position);
        }
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterChangeState += HandleStateChange;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterChangeState -= HandleStateChange;    
    }
}
