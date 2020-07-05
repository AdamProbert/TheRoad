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

    // Start is called before the first frame update
    void Awake()
    {
        characterEventManager = GetComponentInParent<CharacterEventManager>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(lootBoxLayerMask == (lootBoxLayerMask | (1 << other.gameObject.layer)))
        {
            if(other.GetComponentInParent<Lootbox>() == targetBox)
            {
               targetBox.Open(); 
            }
        }
    }

    private void HandleLootboxSelection(Lootbox box)
    {
        targetBox = box;
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterSelectedLootbox += HandleLootboxSelection;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterSelectedLootbox -= HandleLootboxSelection;
    }
}
