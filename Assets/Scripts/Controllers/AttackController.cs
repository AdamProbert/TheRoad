using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AimIK))]
[RequireComponent(typeof(FullBodyBipedIK))]
[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(CharacterData))]
public class AttackController : MonoBehaviour
{
    [SerializeField] BaseWeapon weapon;
    [SerializeField] float attackRotationSpeed;
    [SerializeField] LayerMask blockableLayers;
    [SerializeField] LayerMask enemyLayers;

    Animator anim;
    AimIK aimIK;
    FullBodyBipedIK fbbIK;
    CharacterEventManager characterEventManager;
    FieldOfView viewCone;
    CharacterData characterData;

    Entity currentTarget;
    bool overwatchMode = false;
    
    private void Awake() 
    {
        characterData = GetComponent<CharacterData>();
        viewCone = GetComponentInChildren<FieldOfView>();
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
        viewCone.gameObject.SetActive(false);

        if(weapon.type == WeaponType.AUTOMATICRIFLE)
        {
            anim.SetLayerWeight(1, 0f);
            anim.SetLayerWeight(2, 1f);
        }
        if(weapon.type == WeaponType.ONESHOTRIFLE)
        {
            anim.SetLayerWeight(1, 1f);
            anim.SetLayerWeight(2, 0f);
        }
    }

    private void SetTarget(Entity target)
    {
        if(target != null)
        {
            currentTarget = target;
            aimIK.solver.target = currentTarget.GetAimPointTransform();
            aimIK.solver.IKPositionWeight = Mathf.Lerp(aimIK.solver.IKPositionWeight, 1f, 3f * Time.deltaTime);
        }
        else
        {
            currentTarget = null;
            aimIK.solver.target = null;
            aimIK.solver.IKPositionWeight = Mathf.Lerp(aimIK.solver.IKPositionWeight, 0f, 2f * Time.deltaTime);
            DisableAttack();
        }   
    }

    private void EnableOverwatchMode()
    {
        overwatchMode = true;
        viewCone.gameObject.SetActive(overwatchMode);
        currentTarget = null;
        anim.SetBool("overwatch", true);
    }

    private void DisableOverwatchMode()
    {
        overwatchMode = false;
        viewCone.gameObject.SetActive(false);
        // currentTarget = null;
        anim.SetBool("overwatch", false);
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
        // Prioritise defending self if current target is far away and we're being attack by something closer
        if( 
            (currentTarget != null &&
            Vector3.Distance(transform.position, currentTarget.transform.position) > 5f &&
            ShouldDefendSelf()) ||
            // or if we don't have a target and we need to defend ourselves
            (currentTarget == null &&
            ShouldDefendSelf())
        )
        {
            SetTarget(currentTarget); // Assigned in shouldDefendSelf
            characterEventManager.OnCharacterRequestChangeState(CharacterState.ATTACKING);
        }

        // Then check overwatch
        if(overwatchMode)
        {
            if(!currentTarget || !viewCone.visibleTargets.Contains(currentTarget.transform))
            {
                SetTarget(GetNewTarget());
            }
        }

        // We've chosen the target now get him
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

            if(!overwatchMode)
            {
                // Rotate to face target
                Vector3 direction = currentTarget.GetAimPointPosition() - transform.position;
                Quaternion toRotation = Quaternion.LookRotation(direction);
                toRotation.x = transform.rotation.x;
                toRotation.z = transform.rotation.z;
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, attackRotationSpeed * Time.deltaTime);    
            }
            
        
            bool targetInSight = CanSeeTarget(currentTarget);

            if(targetInSight)
                EnableAttack();
            else
                DisableAttack();
        }
        else
        {
            if(!overwatchMode)
            {
                characterEventManager.OnCharacterRequestChangeState(CharacterState.WAITING);
            }
            SetTarget(null);
        }
    }

    private bool ShouldDefendSelf()
    {
        Collider[] closeEnemies = Physics.OverlapSphere(transform.position, 3f, enemyLayers);
        foreach (Collider c in closeEnemies)
        {
            Entity enemy = c.GetComponentInParent<Entity>();
            if(enemy.alive && CanSeeTarget(enemy))
            {
                currentTarget = enemy;
                return true;
            }
        }
        return false;
    }

    private Entity GetNewTarget()
    {
        Transform closest = null;
        float closestDistance = 9999f;
        foreach (Transform t in viewCone.visibleTargets)
        {
            float dist = Vector3.Distance(t.position, transform.position);
            if(dist < closestDistance)
            {
                closest = t;
                closestDistance = dist;
            }
        }

        if(closest != null)
        {
            return closest.GetComponent<Entity>();
        }
        return null;
    }

    private bool CanSeeTarget(Entity target)
    {
        Vector3 aimPoint = target.GetAimPointPosition();
        if(Physics.Linecast(fbbIK.references.head.position, aimPoint, blockableLayers))
        {
            Debug.DrawLine(fbbIK.references.head.position, aimPoint, Color.green, 1f);
            return true;
        }

        Debug.DrawLine(fbbIK.references.head.position, aimPoint, Color.red, 1f);
        return false;
    }

    //first-order intercept using absolute target position
    public static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )  {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t*(targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    ) {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if(velocitySquared < 0.001f)
            return 0f;
    
        float a = velocitySquared - shotSpeed*shotSpeed;
    
        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude/
            (
                2f*Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }
    
        float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b*b - 4f*a*c;
    
        if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
            float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
                    t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
            if (t1 > 0f) {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            } else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        } else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
    }

    public void OnAnimatorAttackEvent()
    {
        if(currentTarget != null)
        {
            Vector3 shotPosition = currentTarget.GetAimPointPosition();
            float dist = Vector3.Distance(weapon.gunEnd.position, shotPosition);

            if(characterData.getCanLeadShots && weapon.type == WeaponType.ONESHOTRIFLE)
            {
                shotPosition = FirstOrderIntercept
                (
                    weapon.gunEnd.transform.position,
                    Vector3.zero,
                    weapon.projectileForce,
                    shotPosition,
                    currentTarget.GetComponent<Rigidbody>().velocity
                );
            }
            else
            {
                if(dist > viewCone.viewRadius / 3)
                {
                    shotPosition = shotPosition + new Vector3(
                        Random.Range(-characterData.getBaseAccuracy, characterData.getBaseAccuracy),
                        0,
                        0
                    );
                }
            }
            
            Debug.DrawLine(weapon.gunEnd.position, shotPosition, Color.white, 1f);
            weapon.Shoot(shotPosition);
        }
    }

    private void HandleStateChange(CharacterState newState)
    {
        if(newState != CharacterState.ATTACKING)
        {
            currentTarget = null;
        }
        if(newState == CharacterState.OVERWATCHSETUP || newState == CharacterState.OVERWATCH)
        {
            EnableOverwatchMode();
        }
        else
        {
            DisableOverwatchMode();
        }
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterChangeState += HandleStateChange;
        characterEventManager.OnCharacterReceiveNewAttackTarget += SetTarget;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterChangeState -= HandleStateChange;    
        characterEventManager.OnCharacterReceiveNewAttackTarget -= SetTarget;
    }
}
