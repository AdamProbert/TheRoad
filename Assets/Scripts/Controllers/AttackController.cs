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
    Transform currentTarget;
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
        aimIK.solver.transform = weapon.gunEnd;
        fbbIK.solver.leftHandEffector.target = weapon.leftHandPlacement;
        fbbIK.solver.leftHandEffector.positionWeight = 0.8f;
    }

    public void SetTarget(Transform target)
    {
        if(target != null)
        {
            currentTarget = target;
            aimIK.solver.target = currentTarget;
            aimIK.solver.IKPositionWeight = 1;
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
            // Rotate to face target
            Vector3 direction = currentTarget.position - transform.position;
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
        blockingHits = Physics.RaycastAll(weapon.gunEnd.position, weapon.gunEnd.forward, Vector3.Distance(weapon.gunEnd.position, currentTarget.position) + 1f);
        bool hitTarget = false;
        if(blockingHits.Length > 0)
        {
            foreach (RaycastHit hit in blockingHits)
            {
                if(hit.transform == currentTarget)
                {
                    hitTarget = true;
                }

                // First check if hit was right next to player - if so ignore it
                if(hit.distance < 0.5)
                {
                    continue;
                }

                // Then check for blocking areas
                if(blockableLayers == (blockableLayers | (1 << hit.transform.gameObject.layer)))
                {
                    // bool otherRaySeeTarget = false;
                    // RaycastHit otherHit;
                    // Vector3[] othercasts = new []
                    // {
                    //     weapon.gunEnd.position + new Vector3(0f,0.5f,0f),
                    //     weapon.gunEnd.position + new Vector3(0f,-0.5f,0f),
                    //     weapon.gunEnd.position + new Vector3(0.5f,0f,0f),
                    //     weapon.gunEnd.position + new Vector3(-0.5f,0f,0f),
                    // };
                    // // Fire 4 more raycasts to determine if we can see a part of an object
                    // foreach (Vector3 oRay in othercasts)
                    // {
                    //     if(!Physics.Linecast(oRay, currentTarget.position, blockableLayers, out otherHit))
                    //     {
                    //         otherRaySeeTarget = true
                    //     }
                    // }
                    Debug.DrawLine(weapon.gunEnd.position, hit.point, Color.red, 1f);
                    return false;
                }
            }
        }
        
        if(hitTarget)
        {
            Debug.DrawRay(weapon.gunEnd.position, weapon.gunEnd.forward * (Vector3.Distance(weapon.gunEnd.position, currentTarget.position) + 1f), Color.red, 1f);
            return true;
        }
        else
        {
            Debug.DrawRay(weapon.gunEnd.position, weapon.gunEnd.forward * (Vector3.Distance(weapon.gunEnd.position, currentTarget.position) + 1f), Color.yellow, 1f);
            return false;
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
