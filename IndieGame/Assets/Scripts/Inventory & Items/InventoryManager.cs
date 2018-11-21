using UnityEngine;
using F.CharacterStats;

public class InventoryManager : MonoBehaviour
{
    public CharacterStats _health;
    public CharacterStats _mana;
    public CharacterStats _armor;
    public CharacterStats _strength;
    public CharacterStats _intelligence;

    private Inventory _inventory;
    private EquipmentPanel _equipmentPanel;
    private StatPanel _statsPanel;

    private void Awake()
    {
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        _equipmentPanel = GameObject.Find("EquipmentPanel").GetComponent<EquipmentPanel>();

        _statsPanel = GameObject.Find("Stats").GetComponent<StatPanel>();
        _statsPanel.SetStats(_health, _mana, _armor, _strength, _intelligence);
        _statsPanel.UpdateStatValues();

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
                    previousItem.Unequip(this);
                    _statsPanel.UpdateStatValues();
                    _inventory.AddItem(previousItem);
                }
                pItem.Equip(this);
                _statsPanel.UpdateStatValues();
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
            pItem.Unequip(this);
            _statsPanel.UpdateStatValues();
            //If removing the item went fine, add it to the inventory
            _inventory.AddItem(pItem);
        }
    }

    public CharacterStats Health { get { return _health; } set { _health = value; } }
    public CharacterStats Mana { get { return _mana; } set { _mana = value; } }
    public CharacterStats Armor { get { return _armor; } set { _armor = value; } }
    public CharacterStats Strength { get { return _strength; } set { _strength = value; } }
    public CharacterStats Intelligence { get { return _intelligence; } set { _intelligence = value; } }
}
