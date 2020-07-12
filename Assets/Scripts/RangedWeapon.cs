using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RangedWeapon : BaseWeapon
{
    [SerializeField] AudioClip shotSound;
    [SerializeField] ParticleSystem shotFXPrefab;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float fireRate; // This is actually driven by animator
    AudioSource audioSource;
    RaycastHit hit;
    ParticleSystem shotFX;
    float nextFireTime;
    bool shouldShoot;
    Vector3 targetPosition;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        shotFX = Instantiate(shotFXPrefab, gunEnd.position, base.gunEnd.rotation, transform);
        nextFireTime = Time.time + fireRate;
    }
    private void Update()
    {
        if(shouldShoot)
        {

            Vector3 heading = targetPosition - gunEnd.position;
            float distance = heading.magnitude;
            Vector3 accurateDirection = heading / distance; // This is now the normalized direction.
            Vector3 roughDirection = (targetPosition - (transform.root.position + Vector3.up * 1.5f)).normalized;

            // Check if our raycast hit the boy shoot
            if(Time.time > nextFireTime)
            {
                Projectile p = Instantiate(projectilePrefab, gunEnd.position, base.gunEnd.rotation);
                p.transform.position = gunEnd.position;
                p.GetComponent<Rigidbody>().velocity = accurateDirection * projectileForce;
                p.damage = base.damage;
                p.direction = roughDirection;
                audioSource.PlayOneShot(shotSound);
                shotFX.Play();
                nextFireTime = Time.time + fireRate;
                shouldShoot = false;
            }
        } 
    }

    public override void Shoot (Vector3 position)
    {
        // need to do this in update for some reason..
        // I think unity resets the bones at somepoint in the cycle and this fucked it up.
        shouldShoot = true;
        targetPosition = position;
    }
}
