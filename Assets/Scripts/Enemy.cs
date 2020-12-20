using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.StateMachines;
using DG.Tweening;

[RequireComponent(typeof(ZombieData))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FSMOwner))]
[RequireComponent(typeof(AgentLocomotion))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(RagdollController))]
[RequireComponent(typeof(Finder))]
public class Enemy : Entity
{
    [SerializeField] LayerMask lightBlockingLayers;
    ZombieData data;
    Animator anim;
    Finder finder;
    NavMeshAgent agent;
    FSMOwner fSM;
    AgentLocomotion locomotion;
    Collider coll;
    RagdollController ragdoll;

    private void Awake() 
    {
        ragdoll = GetComponent<RagdollController>();
        coll = GetComponent<Collider>();
        locomotion = GetComponent<AgentLocomotion>();
        fSM = GetComponent<FSMOwner>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        data = GetComponent<ZombieData>();
        finder = GetComponent<Finder>();
        base.alegiance = Alegiance.ENEMY;    
    }

    public void PlayRandomSound(List<AudioClip> clips, Vector3 closestListener, float volume)
    {
        int randomIndex = Random.Range(0, clips.Count);
        Vector3 offset = closestListener - transform.position;
        Debug.DrawRay(transform.position, offset, Color.yellow, 5f);
        GameAudioManager.Instance.PlayEnemySoundAtOffsetPosition(clips[randomIndex], offset, volume);
    }

    public void PlaySound(AudioClip clip, Vector3 closestListener, float volume)
    {
        Vector3 offset = closestListener - transform.position;
        Debug.DrawRay(transform.position, offset, Color.yellow, 5f);
        GameAudioManager.Instance.PlayEnemySoundAtOffsetPosition(clip, offset, volume);
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
        finder.enabled = false;

        // Play death sound relative to closest character :|
        List<Character> allCharacters = fSM.blackboard.variables["possibleTargets"].value as List<Character>;
        List<Transform> characterTransforms = new List<Transform>();
        for (int i = 0; i < allCharacters.Count; i++) {
            characterTransforms.Add(allCharacters[i].transform);
        }
        GameAudioManager.Instance.PlayEnemySoundAtOffsetPosition
        (
            data.zombieHitSounds[Random.Range(0, data.zombieHitSounds.Count)],
            Helpers.GetClosestTransform(transform.position, characterTransforms).position,
            1f
        );

        ragdoll.TurnOnRagdollWithForce(finalHitForce);
        transform.DOMoveY(-5f, 30f);
        Destroy(this.gameObject, 30f);
    }
}
