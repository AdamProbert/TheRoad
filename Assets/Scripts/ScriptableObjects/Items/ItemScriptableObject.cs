using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject 
{
    public string itemName;
    public Sprite icon;
    public bool unlimitedUse;
    public int uses;
    public float effectValue;
    public float effectRange;
    public float effectRadius;
    public ParticleSystem effectFX;
    public LayerMask effectedLayers;
}