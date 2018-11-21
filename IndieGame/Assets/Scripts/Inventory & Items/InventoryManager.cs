using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Inventory _inventory;
    private EquipmentPanel _equipmentPanel;

    private void Awake()
    {
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        _equipmentPanel = GameObject.Find("EquipmentPanel").GetComponent<EquipmentPanel>();

        //Call the function when an item from the inventory is right-clicked
        _inventory.OnInventoryItemRightClickEvent += EquipItemFromInventory;
        _equipmentPanel.OnEquipmentPanelItemRightClickEvent += UnequipItemFromEquipmentPanel;
    }

    public void EquipItemFromInventory(Item pItem)
    {
        //Need this function for the right-click event
        if (pItem is Equippable)
        {
            Equip((Equippable)pItem);
        }
    }

    public void UnequipItemFromEquipmentPanel(Item pItem)
    {
        if (pItem is Equippable)
        {
            Unequip((Equippable)pItem);
        }
    }

    public void Equip(Equippable pItem)
    {
        //Try removing the item from inventory
        if (_inventory.RemoveItem(pItem))
        {
            //Add it to the equipment panel
            Equippable previousItem;
            if (_equipmentPanel.AddItem(pItem, out previousItem))
            {
                //If the slot was already taken, return the other item in the inventory
                if (previousItem != null)
                {
                    _inventory.AddItem(previousItem);
                }
            }
            else
            {
                //If for some reason the item couldn't be equipped return it back in the inventory
                _inventory.AddItem(pItem);
            }
        }
    }

    public void Unequip(Equippable pItem)
    {
        //Make sure inventory isn't full first, then try to remove the item
        if (!_inventory.IsFull() && _equipmentPanel.RemoveItem(pItem))
        {
            //If removing the item went fine, add it to the inventory
            _inventory.AddItem(pItem);
        }
    }
}
