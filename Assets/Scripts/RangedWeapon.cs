using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RangedWeapon : BaseWeapon
{
    [SerializeField] AudioClip shotSound;
    [SerializeField] ParticleSystem shotFXPrefab;
    [SerializeField] float fireRate; // This is actually driven by animator
    AudioSource audioSource;
    RaycastHit hit;
    ParticleSystem shotFX;
    float nextFireTime;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        shotFX = Instantiate(shotFXPrefab, gunEnd.transform.position, base.gunEnd.rotation, transform);
        nextFireTime = Time.time + fireRate;
    }

    public override void Shoot (Transform target)
    {
        // Check if our raycast hit the boy shoot
        if(Time.time > nextFireTime)
        {
            if (Physics.Linecast(gunEnd.position, target.position, out hit))
            {
                audioSource.PlayOneShot(shotSound);
                shotFX.Play();
                nextFireTime = Time.time + fireRate;
            }
        }
        
        Debug.DrawLine(gunEnd.position, target.position, Color.red);
        Debug.DrawRay(gunEnd.position, gunEnd.forward * 100f, Color.white);
    }
}
