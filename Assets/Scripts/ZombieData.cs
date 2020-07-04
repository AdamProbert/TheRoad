using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class ZombieData : MonoBehaviour
{
   [SerializeField] public ZombieScriptableObject data;

    private float m_currentHealth;

    private void Start() 
    {
        m_currentHealth = maxHealth;    
    }
    public float maxHealth{
        get {return data.maxHealth;}
        set {}
    }

    public float wanderSpeed{
        get {return data.wanderSpeed;}
        set {}
    }

    public float searchSpeed{
        get {return data.searchSpeed;}
        set {}
    }

    public float chaseSpeed{
        get {return data.chaseSpeed;}
        set {}
    }

    public float maxViewDistance{
        get {return data.maxViewDistance;}
        set {}
    }

    public float maxAwarenessDistance{
        get {return data.maxAwarenessDistance;}
        set {}
    }

    public List<AudioClip> zombieSounds{
        get {return data.zombieSounds;}
        set {}
    }

    public List<AudioClip> zombieHitSounds{
        get {return data.zombieHitSounds;}
        set {}
    }

    public float viewAngle{
        get {return data.viewAngle;}
        set {}
    }

    public Vector3 viewOffset{
        get {return data.viewOffset;}
        set {}
    }

    public List<ParticleSystem> getAllHitEffects{
        get {return data.hitEffects;}
        set {}
    }

    public ParticleSystem getRandomHitEffect{
        get {return data.hitEffects[Random.Range(0, data.hitEffects.Count)];}
        set {}
    }

    public ParticleSystem deathEffect{
        get {return data.deathEffect;}
        set {}
    }
    public float currentHealth{
        get {return m_currentHealth;}
        set {m_currentHealth = value;}
    }

    public float timeBetweenAttacks{
        get {return data.timeBetweenAttacks;}
        set {}
    }

    public float attackDamage{
        get {return data.attackDamage;}
        set {}
    }

    public float zombieAttackAnimCount{
        get {return data.zombieAttackAnimationCount;}
        set {}
    }
}
