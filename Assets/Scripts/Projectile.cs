using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float damage; // Set by weapon
    private int currentPenetrations = 0;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] int penetrationCount;

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Projectile collided with " + other.gameObject.name);
        if(damageableLayers == (damageableLayers | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<Entity>().TakeHit(transform.forward, damage);
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
