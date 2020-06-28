using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AimIK))]
[RequireComponent(typeof(FullBodyBipedIK))]
[RequireComponent(typeof(LookAtIK))]
[RequireComponent(typeof(CharacterEventManager))]
public class AttackController : MonoBehaviour
{
    [SerializeField] BaseWeapon weapon;
    [SerializeField] float attackRotationSpeed;
    Transform currentTarget;
    Animator anim;
    AimIK aimIK;
    FullBodyBipedIK fbbIK;
    LookAtIK lookAtIK;
    CharacterEventManager characterEventManager;

    private void Awake() 
    {
        characterEventManager = GetComponent<CharacterEventManager>();
        fbbIK = GetComponent<FullBodyBipedIK>();
        lookAtIK = GetComponent<LookAtIK>();
        aimIK = GetComponent<AimIK>();
        anim = GetComponent<Animator>();
    }

    private void Start() 
    {   
        // Setup IK for weapon
        aimIK.solver.transform = weapon.gunEnd;
        fbbIK.solver.leftHandEffector.target = weapon.leftHandPlacement;
        fbbIK.solver.leftHandEffector.positionWeight = 0.8f;
    }

    public void SetTarget(Transform target)
    {
        if(target != null)
        {
            currentTarget = target;
            lookAtIK.solver.target = currentTarget;
            aimIK.solver.target = currentTarget;
            aimIK.solver.IKPositionWeight = 1;
            anim.SetBool("Attacking", true);
        }
        else
        {
            currentTarget = null;
            lookAtIK.solver.target = null;
            aimIK.solver.target = null;
            aimIK.solver.IKPositionWeight = 0;
            anim.SetBool("Attacking", false);
        }   
    }

    private void Update() 
    {
        if(currentTarget)
        {
            // Rotate to face
            Vector3 direction = currentTarget.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            toRotation.x = transform.rotation.x;
            toRotation.z = transform.rotation.z;
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, attackRotationSpeed * Time.deltaTime);    
        }
    }

    public void OnAnimatorAttackEvent()
    {
        weapon.Shoot(currentTarget);
    }

    private void HandleStateChange(Character.CharacterState newState)
    {
        if(newState != Character.CharacterState.ATTACKING)
        {
            currentTarget = null;
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
