using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : Singleton<GameAudioManager>
{
	[SerializeField] AudioSource interactableAudioSource;
    [SerializeField] AudioSource enemyAudioSource;
    [SerializeField][Tooltip("The transform containing the audio listener")] Transform listener;
    [SerializeField] DynamicAudioSource sourcePrefab;
    [SerializeField] int maxEnemyAudios;
    int currentEnemyAudios = 0;


	public void PlayEnemySoundAtOffsetPosition(AudioClip clip, Vector3 offset, float volume)
	{
        if(currentEnemyAudios >= maxEnemyAudios)
        {
            return;
        }
        currentEnemyAudios += 1;
        DynamicAudioSource das = Instantiate(sourcePrefab, transform.position, Quaternion.identity, transform);
        das.PlayClipAtPoint(enemyAudioSource, clip, offset, listener, volume);
	}
}
