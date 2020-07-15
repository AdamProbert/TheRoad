using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private float maxHealth;
    [SerializeField] private ParticleSystem hitEffect;
    [Tooltip("Smaller the better")] [SerializeField] private float baseAccuracy;
    [Tooltip("Used for determining trajectory of bullet")][SerializeField] private bool canLeadShots;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sneakSpeed;

    [Header("Overwatch")]
    [SerializeField] private float viewRadius;
    [SerializeField] private float viewAngle;

    [Header("Sneaking")]
    // Awareness something or other?

    [Header("UI")]
    [SerializeField] Sprite portraitImage;

    [Header("Audio")]
    [SerializeField] List<AudioClip> hitSounds;
    [SerializeField] AudioClip deathSound;
    [SerializeField] List<AudioClip> selectionSounds;
    [SerializeField] List<AudioClip> confirmActionSounds;
    [SerializeField] List<AudioClip> confirmMovementSounds;

    private float m_currentHealth;

    private void Awake() {
        FieldOfView fov = GetComponentInChildren<FieldOfView>(true);
        m_currentHealth = maxHealth;
        // Setup field of view
        fov.viewRadius = viewRadius;
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
    public Sprite getPortrait{
        get{return portraitImage;}
        set{}
    }

    public float currentHealth{
        get {return m_currentHealth;}
        set {m_currentHealth = value;}
    }

    public AudioClip getRandomHitSound{
        get {return hitSounds[Random.Range(0, hitSounds.Count)];}
        set {}
    }

    public List<AudioClip> getHitSounds{
        get {return hitSounds;}
        set {}
    }

    public List<AudioClip> getConfirmActionSounds{
        get {return confirmActionSounds;}
        set {}
    }

    public List<AudioClip> getConfirmMoveSounds{
        get {return confirmMovementSounds;}
        set {}
    }

    public List<AudioClip> getSelectionSounds{
        get {return selectionSounds;}
        set {}
    }
    public AudioClip getDeathSound{
        get {return deathSound;}
        set {}
    }

    public float getMoveSpeed{
        get {return moveSpeed;}
        set {}
    }
    
    public float getSneakSpeed{
        get {return sneakSpeed;}
        set {}
    }
}
