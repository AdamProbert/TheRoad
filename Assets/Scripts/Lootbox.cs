using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Lootbox : MonoBehaviour
{
    [SerializeField] ParticleSystem openEffect;
    [SerializeField] public Transform interactionMovePosition;
    Animator anim;

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
        PopulateLoot();    
    } 

    private void PopulateLoot()
    {
        int lootCount = Random.Range(GlobalVarsAccess.Instance.getMinLootPerCrate(), GlobalVarsAccess.Instance.getMaxLootPerCrate());
        for (int i = 0; i < lootCount; i++)
        {
            loot.Add(GlobalVarsAccess.Instance.getRandomLootItem());
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
        Instantiate(openEffect, transform.position, Quaternion.identity);
        anim.enabled = false;
        currentStatus = Status.OPEN;
    }
}
