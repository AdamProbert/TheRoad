using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AimIK))]
[RequireComponent(typeof(FullBodyBipedIK))]
[RequireComponent(typeof(CharacterEventManager))]
public class AttackController : MonoBehaviour
{
    [SerializeField] BaseWeapon weapon;
    [SerializeField] float attackRotationSpeed;
    [SerializeField] LayerMask blockableLayers;
    Entity currentTarget;
    Animator anim;
    AimIK aimIK;
    FullBodyBipedIK fbbIK;
    CharacterEventManager characterEventManager;

    RaycastHit[] blockingHits;

    private void Awake() 
    {
        characterEventManager = GetComponent<CharacterEventManager>();
        fbbIK = GetComponent<FullBodyBipedIK>();
        aimIK = GetComponent<AimIK>();
        anim = GetComponent<Animator>();
    }

    private void Start() 
    {   
        // Setup IK for weapon
        aimIK.solver.transform = weapon.aimDir;
        fbbIK.solver.leftHandEffector.target = weapon.leftHandPlacement;
        fbbIK.solver.leftHandEffector.positionWeight = 0.8f;
    }

    public void SetTarget(Entity target)
    {
        if(target != null)
        {
            currentTarget = target;
            aimIK.solver.target = currentTarget.GetAimPointTransform();
            aimIK.solver.IKPositionWeight = 1f;
            anim.SetBool("Crouch", false);
        }
        else
        {
            currentTarget = null;
            aimIK.solver.target = null;
            aimIK.solver.IKPositionWeight = 0;
            DisableAttack();
            anim.SetBool("Crouch", true);
        }   
    }

    private void EnableAttack()
    {

        anim.SetBool("Attacking", true);
    }

    private void DisableAttack()
    {
        anim.SetBool("Attacking", false);
    }

    private void Update() 
    {
        if(currentTarget)
        {
            if(!currentTarget.alive)
            {
                SetTarget(null);
                return;
            }
            // Disable ik if too close
            if(Vector3.Distance(transform.position, currentTarget.transform.position) < 2f)
            {
                aimIK.solver.IKPositionWeight = 0f;
            }
            else
            {
                aimIK.solver.IKPositionWeight = 1f;
            }
            // Rotate to face target
            Vector3 direction = currentTarget.GetAimPointPosition() - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            toRotation.x = transform.rotation.x;
            toRotation.z = transform.rotation.z;
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, attackRotationSpeed * Time.deltaTime);    
        
            bool targetInSight = CanSeeTarget();

            if(targetInSight)
                EnableAttack();
            else
                DisableAttack();
        }
    }

    private bool CanSeeTarget()
    {
        Vector3 aimPoint = currentTarget.GetAimPointPosition();
        if(Physics.Linecast(weapon.gunEnd.position, aimPoint, blockableLayers))
        {
            Debug.DrawLine(weapon.gunEnd.position, aimPoint, Color.green, 1f);
            return true;
        }

        Debug.DrawLine(weapon.gunEnd.position, aimPoint, Color.red, 1f);
        return false;
    }

    public void OnAnimatorAttackEvent()
    {
        weapon.Shoot(currentTarget.GetAimPointPosition());
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
