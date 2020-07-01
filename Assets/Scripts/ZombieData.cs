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

    public float viewAngle{
        get {return data.viewAngle;}
        set {}
    }

    public Vector3 viewOffset{
        get {return data.viewOffset;}
        set {}
    }

    public ParticleSystem hitEffect{
        get {return data.hitEffect;}
        set {}
    }
    public float currentHealth{
        get {return m_currentHealth;}
        set {m_currentHealth = value;}
    }

}
