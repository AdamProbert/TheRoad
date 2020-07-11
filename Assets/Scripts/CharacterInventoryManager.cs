using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventoryManager : MonoBehaviour
{

    [SerializeField] Transform inventoryUI;
    [SerializeField] LayerMask lootBoxLayerMask;

    CharacterEventManager characterEventManager;

    Lootbox targetBox;
    Lootbox currentBox;

    // Start is called before the first frame update
    void Awake()
    {
        characterEventManager = GetComponentInParent<CharacterEventManager>();
        inventoryUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(lootBoxLayerMask == (lootBoxLayerMask | (1 << other.gameObject.layer)))
        {
            if(other.GetComponentInParent<Lootbox>() == targetBox)
            {
               targetBox.Open(); 
            }
            else
            {
                currentBox = other.GetComponentInParent<Lootbox>();
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(lootBoxLayerMask == (lootBoxLayerMask | (1 << other.gameObject.layer)))
        {
            if(other.GetComponentInParent<Lootbox>() == currentBox)
            {
               currentBox = null;
            }
        }
    }

    private void HandleLootboxSelection(Lootbox box)
    {
        targetBox = box;
        if(currentBox && currentBox == targetBox)
        {
            currentBox.Open();
        }
    }

    private void HandleCharacterSelect(bool isISelected)
    {
        inventoryUI.gameObject.SetActive(isISelected);
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterSelectedLootbox += HandleLootboxSelection;
        characterEventManager.OnCharacterSelected += HandleCharacterSelect;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterSelectedLootbox -= HandleLootboxSelection;
        characterEventManager.OnCharacterSelected -= HandleCharacterSelect;
    }
}
