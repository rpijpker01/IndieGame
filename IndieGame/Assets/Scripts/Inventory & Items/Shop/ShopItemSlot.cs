using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSlot : ItemSlot
{
    public override bool CanReceiveItem(Item pItem)
    {
        return base.CanReceiveItem(pItem);
    }
}