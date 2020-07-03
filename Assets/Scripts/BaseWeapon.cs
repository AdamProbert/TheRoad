using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    AUTOMATICRIFLE,
    ONESHOTRIFLE
}
public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] public Transform leftHandPlacement;
    [SerializeField] public Transform gunEnd;
    [SerializeField] public Transform aimDir;
    [SerializeField] public float damage;
    [SerializeField] public WeaponType type;
    [SerializeField] public float projectileForce;
    public abstract void Shoot(Vector3 position);

}
