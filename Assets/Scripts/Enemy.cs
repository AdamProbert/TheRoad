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
public class Enemy : Entity
{
    ZombieData data;
    Animator anim;
    NavMeshAgent agent;
    FSMOwner fSM;
    AgentLocomotion locomotion;
    Collider coll;
    RagdollController ragdoll;
    AudioSource audioSource;

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
        base.alegiance = Alegiance.ENEMY;    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeHit(Vector3 direction, float damage)
    {
        Instantiate(data.getRandomHitEffect, base.GetAimPointPosition(), Quaternion.LookRotation(direction));
        
        audioSource.PlayOneShot(
            data.zombieHitSounds[Random.Range(0, data.zombieHitSounds.Count)],
            1f
        );
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
        ragdoll.TurnOnRagdollWithForce(finalHitForce);
    }
}
