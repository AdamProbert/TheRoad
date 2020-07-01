using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CharacterData : MonoBehaviour
{
   [SerializeField] public CharacterScriptableObject cData;

    private float m_currentHealth;

    private void Start() 
    {
        m_currentHealth = maxHealth;    
    }
    public float maxHealth{
        get {return cData.maxHealth;}
        set {}
    }

    public ParticleSystem hitEffect{
        get {return cData.hitEffect;}
        set {}
    }

    public float currentHealth{
        get {return m_currentHealth;}
        set {m_currentHealth = value;}
    }

}
