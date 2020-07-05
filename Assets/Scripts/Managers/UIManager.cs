using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Portraits")]
    [SerializeField] CharacterPortrait portraitPrefab;
    [SerializeField] VerticalLayoutGroup portraitGroup;

    Dictionary <Character, CharacterPortrait> portraitMapping = new Dictionary<Character, CharacterPortrait>();

    public void Register(Character character, Sprite icon)
    {
        CharacterPortrait cp = Instantiate(portraitPrefab, transform.position, Quaternion.identity, portraitGroup.transform);
        cp.Initialize(character, icon);
        portraitMapping.Add(character, cp);
    }

    public void UpdateHealth(Character character, float maxHealth, float health)
    {
        portraitMapping[character].SetHealth(health, maxHealth);
    }

    private void RemovePortrait(Character character)
    {
        Destroy(portraitMapping[character].gameObject);
        portraitMapping.Remove(character);
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnCharacterDied += RemovePortrait;    
    }

    private void OnDisable() 
    {
        if(PlayerEventManager.Instance != null)
        {
            PlayerEventManager.Instance.OnCharacterDied -= RemovePortrait;    
        }
    }
}
