using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (CharacterEventManager))]
[RequireComponent(typeof(LookAtIK))]
public class AgentLocomotion : MonoBehaviour {
    Animator anim;
    NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    CharacterEventManager characterEventManager;
    LookAtIK lookAtIK;

    public bool moving = false;

    private void Awake() 
    {
        anim = GetComponent<Animator> ();
        agent = GetComponent<NavMeshAgent> ();    
        characterEventManager = GetComponent<CharacterEventManager>();
        lookAtIK = GetComponent<LookAtIK>();
    }

    void Start ()
    {
        // Don’t update position automatically
        agent.updatePosition = false;
    }
    
    void Update ()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > 0.05f;

        // Update animation parameters
        anim.SetBool("ShouldMove", shouldMove);
        anim.SetFloat ("velx", velocity.x);
        anim.SetFloat ("vely", velocity.y);

        lookAtIK.solver.IKPosition = agent.steeringTarget + transform.forward;

        moving = shouldMove; // Previous frame state
    }

    void OnAnimatorMove ()
    {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }
}