using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Throwable : Item
{
    public override void UseItem(Character character)
    {
        character.UseItem(this);
    }
}
