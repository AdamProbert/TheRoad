﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    
    [SerializeField] public Transform leftHandPlacement;
    [SerializeField] public Transform gunEnd;
    [SerializeField] public Transform aimDir;
    [SerializeField] public float damage;

    public abstract void Shoot(Vector3 position);

}
