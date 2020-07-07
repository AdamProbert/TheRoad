using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Consumable : Item
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseItem(Character character)
    {
        character.UseConsubale(base.itemData.effectValue);
    }
}
