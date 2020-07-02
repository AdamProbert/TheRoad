using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZombieData", menuName = "ScriptableObjects/Zombie", order = 1)]
public class ZombieScriptableObject : ScriptableObject 
{
    public float maxHealth;
    public float wanderSpeed;
    public float searchSpeed;
    public float chaseSpeed;
    public float maxViewDistance;
    public float maxAwarenessDistance;
    public float viewAngle;
    public Vector3 viewOffset;
    public List<AudioClip> zombieSounds;
    public List<AudioClip> zombieHitSounds;

    public ParticleSystem hitEffect;
}