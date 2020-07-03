using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterScriptableObject : ScriptableObject 
{
    public string characterName;
    public float maxHealth;
    public ParticleSystem hitEffect;
    [Tooltip("Smaller the better")] public float baseAccuracy;
    [Tooltip("Used for determining trajectory of bullet")]public bool canLeadShots;
    
    [Header("Overwatch")]
    public float viewRadius;
    public float viewAngle;
}