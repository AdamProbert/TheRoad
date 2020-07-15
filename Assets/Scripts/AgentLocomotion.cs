using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]

public class AgentLocomotion : MonoBehaviour {
    Animator anim;
    NavMeshAgent agent;
    CharacterEventManager characterEventManager;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    public bool moving = false;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        characterEventManager = GetComponent<CharacterEventManager>();
    }

    void Start ()
    {
        // Don’t update position automatically
        agent.updatePosition = false;
    }
    
    void Update ()
    {
        // Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // // Map 'worldDeltaPosition' to local space
        // float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        // float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        // Vector2 deltaPosition = new Vector2 (dx, dy);

        // // Low-pass filter the deltaMove
        // float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        // smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // // Update velocity if time advances
        // if (Time.deltaTime > 1e-5f)
        //     velocity = smoothDeltaPosition / Time.deltaTime;

        

        Vector2 agentVelocity = AgentVelocityToVector2DInput();

        bool shouldMove = agentVelocity.magnitude > 0.5f && agent.remainingDistance > 0.05f;

        // Update animation parameters
        anim.SetBool("ShouldMove", shouldMove);
        anim.SetFloat ("velx", agentVelocity.x);
        anim.SetFloat ("vely", agentVelocity.y);
        anim.SetFloat ("speed", agent.speed);

        moving = shouldMove; // Previous frame state
    }

     public Vector2 AgentVelocityToVector2DInput()
    {
         float xValue;
         float yValue;
         // Get the NavMeshAgent's desired velocity direction relative from it's actual position
         Vector3 desiredVelocityRelativeToAgent = agent.transform.InverseTransformDirection(agent.desiredVelocity);
         // Normalize the vector so it doesn't have a magnitude beyond 1.0f
         desiredVelocityRelativeToAgent.Normalize();
         // X value will be the X value of the vector
         xValue = desiredVelocityRelativeToAgent.x;
         // Y value will be the Z value of the vector
         yValue = desiredVelocityRelativeToAgent.z;
         // It's worth noting that you should scale this 2D vector by a desired speed on a scale of 0 - 1
         return new Vector2(xValue, yValue);
    }

    void OnAnimatorMove ()
    {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }

    void HandleStateChange(CharacterState newState)
    {
        if(newState == CharacterState.DEAD)
        {
            agent.enabled = false;
            this.enabled = false;
        }
    }

    void HandleStealthStateChange(CharacterStealthState newState)
    {
        if(newState == CharacterStealthState.SNEAKING)
        {
            anim.SetBool("Sneaking", true);
        }
        else
        {
            anim.SetBool("Sneaking", false);
        }
    }

    private void OnEnable() 
    {
        if(characterEventManager != null)
        {
            characterEventManager.OnCharacterChangeState += HandleStateChange;
            characterEventManager.OnCharacterChangeStealthState += HandleStealthStateChange;
        }
        
    }

    private void OnDisable() 
    {
        if(characterEventManager != null)
        {
            characterEventManager.OnCharacterChangeState -= HandleStateChange;
            characterEventManager.OnCharacterChangeStealthState -= HandleStealthStateChange;
        }
    }
}