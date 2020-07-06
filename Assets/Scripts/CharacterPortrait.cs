using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterPortrait  : MonoBehaviour, IPointerClickHandler 
{

    [SerializeField] Image selectionCircle;
    [SerializeField] Image healthBar;
    [SerializeField] Image portrait;
    [SerializeField] Color selectionColor;
    [SerializeField] Color defaultColor;

    Character character;
    PlayerInputManager playerInputManager;

    public void Initialize(Character character, Sprite image)
    {
        this.character = character;
        portrait.sprite = image;
        playerInputManager = GameObject.FindObjectOfType<PlayerInputManager>();
    }

    public void Activate()
    {
        selectionCircle.color = selectionColor;
    }
    public void DeActivate()
    {
        selectionCircle.color = defaultColor;
    }

    public void HandleClickCharacter(Character c)
    {
        if(c == character)
        {
            Activate();
        }
        else
        {
            DeActivate();
        }
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = (currentHealth / maxHealth);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerInputManager.HandleClickEntity(character);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }


    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnPlayerSelectCharacter += HandleClickCharacter;
    }

    private void OnDisable() 
    {
        if(PlayerEventManager.Instance != null)
        {
            PlayerEventManager.Instance.OnPlayerSelectCharacter -= HandleClickCharacter;    
        }
    }
}