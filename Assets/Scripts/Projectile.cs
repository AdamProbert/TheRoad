using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float damage; // Set by weapon
    public Vector3 direction; // Set by weapon
    private int currentPenetrations = 0;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] int penetrationCount;

    private void OnEnable() 
    {
        Destroy(this.gameObject, 5f);    
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Projectile collided with " + other.gameObject.name);
        if(damageableLayers == (damageableLayers | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<Entity>().TakeHit(direction, damage);
            currentPenetrations += 1;
            if(currentPenetrations > penetrationCount)
            {
                Destroy(this.gameObject);    
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
