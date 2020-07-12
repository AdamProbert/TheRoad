using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField] public enum CharacterAudioCategory
{
	SELECTION,
	HIT,
	DEATH,
	MOVE,
	ACTION
}

[RequireComponent(typeof(AudioSource))]

public class CharacterAudioManager : Singleton<CharacterAudioManager>
{
	[Header("THESE ARRAYS MUST BE THE SAME LENGTH. Maps max sounds to categories")]
	[SerializeField][Tooltip("These should be unique")]
	List<CharacterAudioCategory> categoryList;
	[SerializeField] List<int> maxSoundsPerCategory;

	Dictionary<CharacterAudioCategory, int> maxsoundsMap = new Dictionary<CharacterAudioCategory, int>();
	Dictionary<CharacterAudioCategory, int> soundsMap = new Dictionary<CharacterAudioCategory, int>();
	Dictionary<Character, int> individaulSoundMap = new Dictionary<Character, int>();

    AudioSource audioSource;
    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
		for (int i = 0; i < categoryList.Count; i++)
		{
			maxsoundsMap.Add(categoryList[i], maxSoundsPerCategory[i]);
			soundsMap.Add(categoryList[i], 0);
		}
    }

	private bool checkSoundMaps(Character character, CharacterAudioCategory category)
	{
		if(!individaulSoundMap.ContainsKey(character))
		{
			individaulSoundMap[character] = 0;
		}

		if(soundsMap[category] >= maxsoundsMap[category] || individaulSoundMap[character] > 0)
		{
			return false;
		}
		
		soundsMap[category] += 1;
		individaulSoundMap[character] += 1;
		return true;
	}

	// Play a random clip from an array, and randomize the pitch slightly.
	public void PlayRandoSound(List<AudioClip> clips, Character character, CharacterAudioCategory category, bool mustPlay = false)
	{
		int randomIndex = Random.Range(0, clips.Count);
		if(mustPlay)
		{
			audioSource.PlayOneShot(clips[randomIndex]);
			return;
		}

		if(checkSoundMaps(character, category))
		{
			StartCoroutine(playCharacterSound(clips[randomIndex], character, category));
		}
	}

    public void PlaySound(AudioClip clip, Character character, CharacterAudioCategory category, bool mustPlay = false)
	{
		if(mustPlay)
		{
			audioSource.PlayOneShot(clip);
		}
		if(checkSoundMaps(character, category))
		{
			StartCoroutine(playCharacterSound(clip, character, category));
		}
	}

	IEnumerator playCharacterSound(AudioClip clip, Character character, CharacterAudioCategory category)
	{
		audioSource.PlayOneShot(clip);
		yield return new WaitForSeconds(clip.length);
		soundsMap[category] -= 1;
		individaulSoundMap[character] -= 1;
	}
}
