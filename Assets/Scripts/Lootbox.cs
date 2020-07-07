using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Lootbox : MonoBehaviour
{
    [SerializeField] ParticleSystem openEffect;
    [SerializeField] public Transform interactionMovePosition;
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
        foreach (Item item in loot)
        {
            item.gameObject.SetActive(true);
            Vector2 xz = Random.insideUnitCircle * .5f;
            Vector3 lootPosition = new Vector3(
                xz.x, 0, xz.y
            ) + transform.position;
            
            item.transform.DOJump(lootPosition, 5f, 1, 1,false);       
        }

        Instantiate(openEffect, transform.position, Quaternion.identity);
        anim.enabled = false;
        currentStatus = Status.OPEN;
        GetComponent<Collider>().enabled = false;
    }
}
