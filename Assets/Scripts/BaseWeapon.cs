using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    
    [SerializeField] public Transform leftHandPlacement;
    [SerializeField] public Transform gunEnd;

    public abstract void Shoot(Transform target);

}
