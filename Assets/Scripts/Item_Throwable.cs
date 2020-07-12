using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Throwable : Item
{
    [SerializeField] LayerMask blocking;
    public override void UseItem(Character character)
    {
        character.UseItem(this);
    }

    public override void ActiveEffect()
    {
        Instantiate(effectFX, transform.position, Quaternion.identity);
        if(effectRadius > 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius, effectedLayers);
            foreach (var hitCollider in hitColliders)
            {
                Entity entity = hitCollider.GetComponentInParent<Entity>();
                if(!entity)
                {
                    return;
                }

                RaycastHit hit;
                if(Physics.Linecast(transform.position, entity.GetAimPointPosition(), out hit, blocking)) 
                {
                    if(hit.collider == hitCollider)
                    {
                        Debug.DrawLine(transform.position, entity.GetAimPointPosition(), Color.red, 5f);
                        Vector3 dir = (entity.GetAimPointPosition() - transform.position).normalized;
                        entity.TakeHit(dir, effectValue);
                    }
                }                
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, effectRadius);
    }
}
