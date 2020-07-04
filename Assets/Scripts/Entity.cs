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

    protected bool alive = true;

    public Vector3 GetAimPointPosition()
    {
        return aimPoint.position;
    }

    public Transform GetAimPointTransform()
    {
        return aimPoint;
    }
    
    public bool isAlive()
    {
        return alive;
    }

    public abstract void TakeHit(Vector3 direction, float damage);
}
