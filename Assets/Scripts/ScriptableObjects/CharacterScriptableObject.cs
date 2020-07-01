using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterScriptableObject : ScriptableObject 
{
    public float maxHealth;
    public ParticleSystem hitEffect;

}