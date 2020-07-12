using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Lootbox : MonoBehaviour
{
    [SerializeField] ParticleSystem openEffect;
    [SerializeField] public Transform interactionMovePosition;
    [SerializeField] List<Transform> lootPositions;
    Animator anim;
    bool showingInventory;

    public enum Status
    {
        CLOSED,
        OPEN,
        EMPTY
    }

    private List<Item> loot = new List<Item>();

    public Status currentStatus = Status.CLOSED;

    void Start() 
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        // inventoryUI.transform.position = transform.position;
        PopulateLoot();    
    } 

    private void PopulateLoot()
    {
        int lootCount = Random.Range(GlobalVarsAccess.Instance.getMinLootPerCrate(), GlobalVarsAccess.Instance.getMaxLootPerCrate());
        for (int i = 0; i < lootCount; i++)
        {
            Item item = Instantiate(GlobalVarsAccess.Instance.getRandomLootItem(), transform.position, Quaternion.identity);
            item.gameObject.SetActive(false);
            loot.Add(item);
        }
    }

    public void Open()
    {
        if(currentStatus == Status.CLOSED)
        {
            anim.enabled = true;
        }
    }

    public void OnBoxOpen()
    {
        int lootPosIndex = 0;
        foreach (Item item in loot)
        {
            item.gameObject.SetActive(true);            
            item.transform.DOJump(lootPositions[lootPosIndex].position, 5f, 1, 1,false);       
            lootPosIndex += 1;
            if(lootPosIndex >= lootPositions.Count)
            {
                lootPosIndex = 0;
            }
        }

        Instantiate(openEffect, transform.position, Quaternion.identity);
        anim.enabled = false;
        currentStatus = Status.OPEN;
        GetComponent<Collider>().enabled = false;
    }
}
