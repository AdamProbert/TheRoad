using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    List<Collider> ragdollParts = new List<Collider>();
    CapsuleCollider playerCollider;
    Rigidbody playerRB;
    Animator playerAnimator;

    private void Awake() 
    {
        GetRagdollParts();
        TurnOffRagdoll();
        playerCollider = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    private void GetRagdollParts()
    {
        Collider[] allColliders = this.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in allColliders)
        {
            if(c.gameObject != this.gameObject)
            {
                ragdollParts.Add(c);
            }
        }
    }

    private void TurnOffRagdoll()
    {
        if(playerCollider)
        {
            playerCollider.enabled = false;
        }
        foreach (Collider c in ragdollParts)
        {
            
            c.gameObject.layer = LayerMask.NameToLayer("RagdollLimb");
            c.isTrigger = true;
            c.enabled = false;
            c.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void TurnOnRagdollWithForce(Vector3 force)
    {
        TurnOnRagdoll();
        foreach (Collider c in ragdollParts)
        {
            c.attachedRigidbody.AddForce(force);
        }
    }

    public void TurnOnRagdoll()
    {
        if(playerAnimator)
        {
            // playerAnimator.avatar = null;
            playerAnimator.enabled = false;    
        }

        foreach (Collider c in ragdollParts)
        {
            c.enabled = true;
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }
}

