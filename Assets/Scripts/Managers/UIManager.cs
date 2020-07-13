using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Portraits")]
    [SerializeField] CharacterPortrait portraitPrefab;
    [SerializeField] VerticalLayoutGroup portraitGroup;

    [Header("Tooltips")]
    [SerializeField] Tooltip tooltip;

    Dictionary <Character, CharacterPortrait> portraitMapping = new Dictionary<Character, CharacterPortrait>();

    public void ShowTooltip(string text)
    {
        tooltip.enabled = true;
        tooltip.Activate(text);
    }

    public void HideTooltip()
    {
        tooltip.Deactivate();
    }

    public void Register(Character character, Sprite icon)
    {
        CharacterPortrait cp = Instantiate(portraitPrefab, transform.position, Quaternion.identity, portraitGroup.transform);
        cp.Initialize(character, icon);
        portraitMapping.Add(character, cp);
    }

    public void UpdateHealth(Character character, float maxHealth, float health)
    {
        if(portraitMapping.ContainsKey(character))
        {
            portraitMapping[character].SetHealth(health, maxHealth);
        }
    }

    private void RemovePortrait(Character character)
    {
        if(portraitMapping.ContainsKey(character))
        {
            Destroy(portraitMapping[character].gameObject);
            portraitMapping.Remove(character);
        }
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
