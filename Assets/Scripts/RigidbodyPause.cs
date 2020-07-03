using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPause : MonoBehaviour
{
    [SerializeField] bool isRagdoll;

    Rigidbody rb;
    Vector3 savedVelocity;
    Vector3 savedAngularVelocity;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();    
    }

    void OnPauseGame() {
        // savedVelocity = rb.velocity;
        // savedAngularVelocity = rb.angularVelocity;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.velocity = Vector3.zero;
        if(isRagdoll)
        {
            foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
                rb.velocity = Vector3.zero;
            }
        }
        Debug.Log("Rigidbody paused");
    }

    void OnResumeGame() {
        if(isRagdoll)
        {
            foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.velocity = Vector3.zero;
            }
        }
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.velocity = Vector3.zero;
        // rb.AddForce( savedVelocity, ForceMode.VelocityChange );
        // rb.AddTorque( savedAngularVelocity, ForceMode.VelocityChange );
        Debug.Log("Rigidbody Resumed");
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPauseTime += OnPauseGame;
        PlayerEventManager.Instance.OnResumeTime += OnResumeGame;
    }

    private void OnDisable() 
    {
        PlayerEventManager.Instance.OnPauseTime -= OnPauseGame;
        PlayerEventManager.Instance.OnResumeTime -= OnResumeGame;    
    }
}
