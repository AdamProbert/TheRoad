using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [SerializeField] public Transform gunEnd;
    [SerializeField] public Transform leftHandPlacement;

    RaycastHit hit;

    public void Shoot (Transform target) 
    {
        // Check if our raycast has hit anything
        if (Physics.Linecast(gunEnd.position, target.position, out hit))
        {
            Debug.Log("We hit: " + hit.transform.gameObject.name);       
        }
        Debug.DrawLine(gunEnd.position, target.position, Color.red);
        Debug.DrawRay(gunEnd.position, gunEnd.forward * 100f, Color.white);
    }
}
