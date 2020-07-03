using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private float maxHealth;
    [SerializeField] private ParticleSystem hitEffect;
    [Tooltip("Smaller the better")] [SerializeField] private float baseAccuracy;
    [Tooltip("Used for determining trajectory of bullet")][SerializeField] private bool canLeadShots;
    
    [Header("Overwatch")]
    [SerializeField] private float viewRadius;
    [SerializeField] private float viewAngle;
    private float m_currentHealth;

    private void Awake() {
        FieldOfView fov = GetComponentInChildren<FieldOfView>(true);
        m_currentHealth = maxHealth;
        // Setup field of view
        Debug.Log("ViewRadius: " + viewRadius);
        Debug.Log("Field of view: " + fov.viewRadius);
        fov.viewRadius = viewRadius;
        Debug.Log("Field of view after set: " + fov.viewRadius);
        fov.viewAngle = viewAngle;
    }
    
    public float getMaxHealth{
        get {return maxHealth;}
        set {}
    }

    public ParticleSystem getHitEffect{
        get {return hitEffect;}
        set {}
    }

    public bool getCanLeadShots{
        get {return canLeadShots;}
        set {}
    }

    public float getBaseAccuracy{
        get {return baseAccuracy;}
        set {}
    }

    public float currentHealth{
        get {return m_currentHealth;}
        set {m_currentHealth = value;}
    }
}
