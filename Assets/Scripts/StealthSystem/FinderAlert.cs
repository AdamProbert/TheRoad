using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinderAlert : MonoBehaviour
{
    // A behavior used for any kind of alerts a finder may need to receive.
    // For example a grenade going off, or a weapon firing.

    [SerializeField] bool alertOnCreate;
    [SerializeField] float radius;
    [SerializeField] LayerMask finders;

    public void TriggerAlert()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, finders);
        foreach (Collider col in cols)
        {
            Finder f = col.GetComponentInParent<Finder>();
            f.ReceiveAlert(transform.position);
        }
    }

    private void OnEnable() 
    {
        if(alertOnCreate)
        {
            TriggerAlert();
        }    
    }
}
