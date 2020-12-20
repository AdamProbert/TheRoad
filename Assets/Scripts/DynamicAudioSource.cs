using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicAudioSource : MonoBehaviour
{
    AudioSource source;
    Transform listener;
    Vector3 offset;
    
    // This is a confusing class.. but basically you pass it an audiosource to copy the values from
    // A audio clip to play
    // A position to play that clip at
    // And the scene audio listener transform.
    // Then in the update it update this audio source to always be the same distance from the listener
    // as originally requested.
    // This is needed because we can only have one listener per scene and this emulates what the characters
    // hear from their perspective.

    private void Awake() 
    {
        source = GetComponent<AudioSource>();    
    }

    // Update position relative to offset requested.
    private void Update()
    {
        if(listener)
        {
            Vector3 worldPos = listener.position - offset;
            transform.position = worldPos;    
        }
    }

    // copies audiosource properties to temp audiosource for playing at a position
    public void  PlayClipAtPoint(
        AudioSource audioSource,
        AudioClip clip,
        Vector3 offset,
        Transform listener,
        float volume
    )
    {
        this.listener = listener;
        this.offset = offset;
        Vector3 worldPos = listener.position - offset;
        transform.position = worldPos;
        source.clip = clip;
        source.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        source.mute = audioSource.mute;
        source.bypassEffects = audioSource.bypassEffects;
        source.bypassListenerEffects = audioSource.bypassListenerEffects;
        source.bypassReverbZones = audioSource.bypassReverbZones;
        source.playOnAwake = audioSource.playOnAwake;
        source.loop = audioSource.loop;
        source.priority = audioSource.priority;
        source.volume = volume;
        source.pitch = audioSource.pitch;
        source.panStereo = audioSource.panStereo;
        source.spatialBlend = audioSource.spatialBlend;
        source.reverbZoneMix = audioSource.reverbZoneMix;
        source.dopplerLevel = audioSource.dopplerLevel;
        source.rolloffMode = audioSource.rolloffMode;
        source.minDistance = audioSource.minDistance;
        source.spread = audioSource.spread;
        source.maxDistance = audioSource.maxDistance;
        source.Play(); // start the sound
        Destroy(this.gameObject, source.clip.length); // destroy object after clip duration (this will not account for whether it is set to loop)
    }
}
