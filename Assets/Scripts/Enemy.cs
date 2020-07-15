using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.StateMachines;

[RequireComponent(typeof(ZombieData))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FSMOwner))]
[RequireComponent(typeof(AgentLocomotion))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(RagdollController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class Enemy : Entity
{
    [SerializeField] LayerMask lightBlockingLayers;
    ZombieData data;
    Animator anim;
    NavMeshAgent agent;
    FSMOwner fSM;
    AgentLocomotion locomotion;
    Collider coll;
    RagdollController ragdoll;
    AudioSource audioSource;
    LineRenderer sightLine;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        ragdoll = GetComponent<RagdollController>();
        coll = GetComponent<Collider>();
        locomotion = GetComponent<AgentLocomotion>();
        fSM = GetComponent<FSMOwner>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        data = GetComponent<ZombieData>();
        sightLine = GetComponent<LineRenderer>();
        base.alegiance = Alegiance.ENEMY;    
        StopSightLine();
    }

    public void StopSightLine()
    {
        sightLine.enabled = false;
    }

    public void DrawSightLineToTarget(Character target, float visibility)
    {
        if(target.GetCurrentStealthState() == CharacterStealthState.SNEAKING)
        {
            sightLine.enabled = true;
            Color c = new Color(255,255,255, visibility);
            sightLine.startColor = c;
            sightLine.endColor = c;
            sightLine.positionCount = 2;
            sightLine.SetPosition(0, base.GetAimPointPosition());
            sightLine.SetPosition(1, target.GetAimPointPosition());
        }
        else
        {
            StopSightLine();
        }
    }

    public float CalculateChanceToSeeTarget(Character target)
    {
        float visibility = target.GetCurrentVisibilty();
        RaycastHit hit;
        
        // Factor in visible to enemy
        if(Physics.Linecast(target.GetAimPointPosition(), GetAimPointPosition(), out hit, lightBlockingLayers))
        {
            if(hit.transform.root == this.transform.root)
            {
                visibility += 10f;
            }
            else
            {
                visibility -= 10f;
            }
        } 

        // Factor in distance
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance < 5f)
        {
            visibility += 10f;
        }
        else if(distance < 10f)
        {
            visibility += 5f;
        }
        else
        {
            visibility -= 10f;
        }

        return visibility;

    }

    public override void TakeHit(Vector3 direction, float damage)
    {
        Instantiate(data.getRandomHitEffect, base.GetAimPointPosition(), Quaternion.LookRotation(direction));
        
        if(alive)
        {
            data.currentHealth -= damage;
            if(data.currentHealth <= damage)
            {
                Die(direction*damage);
            }
        }
   }

    private void Die(Vector3 finalHitForce)
    {
        alive = false;
        agent.enabled = false;
        fSM.enabled = false;
        locomotion.enabled = false;
        coll.enabled = false;
        audioSource.PlayOneShot(
            data.zombieHitSounds[Random.Range(0, data.zombieHitSounds.Count)],
            1f
        );
        ragdoll.TurnOnRagdollWithForce(finalHitForce);
    }
}
