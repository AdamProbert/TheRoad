using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alegiance
{
    FRIENDLY,
    ENEMY
}
public abstract class Entity : MonoBehaviour
{
    [SerializeField] Transform aimPoint;
    public Alegiance alegiance;
    public float remainingHealth;

    public bool alive = true;

    public Vector3 GetAimPointPosition()
    {
        return aimPoint.position;
    }

    public Transform GetAimPointTransform()
    {
        return aimPoint;
    }

    public abstract void TakeHit(Vector3 direction, float damage);
}
