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
public class Enemy : Entity
{
    ZombieData data;
    Animator anim;
    NavMeshAgent agent;
    FSMOwner fSM;
    AgentLocomotion locomotion;
    Collider coll;

    private void Awake() 
    {
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
        Instantiate(data.hitEffect, base.GetAimPointPosition(), Quaternion.LookRotation(direction));
        if(alive)
        {
            data.currentHealth -= damage;
            if(data.currentHealth <= damage)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        alive = false;
        anim.SetTrigger("Die");
        agent.enabled = false;
        fSM.enabled = false;
        locomotion.enabled = false;
        coll.enabled = false;
    }
}
