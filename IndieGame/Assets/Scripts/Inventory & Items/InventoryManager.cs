using UnityEngine;
using F.CharacterStats;

public class InventoryManager : MonoBehaviour
{
    public CharacterStats _health;
    public CharacterStats _mana;
    public CharacterStats _armor;
    public CharacterStats _strength;
    public CharacterStats _intelligence;

    private PlayerCoins _playerCoins;

    private Inventory _inventory;
    private EquipmentPanel _equipmentPanel;
    private StatPanel _statsPanel;

    private ShopInventory _shopInventory;

    private void Awake()
    {
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        _equipmentPanel = GameObject.Find("EquipmentPanel").GetComponent<EquipmentPanel>();

        _shopInventory = GameObject.Find("ShopPanel").GetComponent<ShopInventory>();
        _playerCoins = GameObject.Find("Coins").GetComponent<PlayerCoins>();

        _statsPanel = GameObject.Find("Stats").GetComponent<StatPanel>();
        _statsPanel.SetStats(_health, _mana, _armor, _strength, _intelligence);
        _statsPanel.UpdateStatValues();

        //Call the function when an item from the inventory is right-clicked
        _inventory.OnInventoryItemRightClickEvent += EquipItemFromInventory;
        _equipmentPanel.OnEquipmentPanelItemRightClickEvent += UnequipItemFromEquipmentPanel;

        _inventory.OnInventoryItemRightClickEvent += Sell;
        _shopInventory.OnShopInventoryItemRightClickEvent += Buy;
    }

    public void EquipItemFromInventory(Item pItem)
    {
        if (ShopKeeper.playerIsInShop) return;

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

    public void Sell(Item pItem)
    {
        if (!ShopKeeper.playerIsInShop) return;
        if (_shopInventory == null)
            _shopInventory = GameObject.Find("ShopInventory").GetComponent<ShopInventory>();

        if (_inventory.RemoveItem(pItem))
        {
            if (!_shopInventory.AddItem(pItem))
            {
                _inventory.AddItem(pItem);
                GameController.errorMessage.DisplayMessage("Shop is full!");
            }
            else
            {
                //Give player coins for selling the item
                Equippable e = pItem as Equippable;
                if (e != null)
                {
                    GameController.errorMessage.DisplayMessage(e.Name + "\nhas been sold!");
                    _playerCoins.AddCoins(e.Value);
                }
            }
        }
    }

    public void Buy(Item pItem)
    {
        if (_shopInventory == null)
            _shopInventory = GameObject.Find("ShopInventory").GetComponent<ShopInventory>();

        if (_shopInventory.RemoveItem(pItem))
        {
            Equippable e = pItem as Equippable;
            if (e != null)
            {
                if (_playerCoins.TakeCoins(Mathf.RoundToInt(e.Value * 1.25f)))
                {
                    if (!_inventory.AddItem(pItem))
                    {
                        _shopInventory.AddItem(pItem);
                        GameController.errorMessage.DisplayMessage("Inventory is full!");
                    }
                    else
                    {
                        GameController.errorMessage.DisplayMessage(e.Name + "\nhas been purchased!");
                    }
                }
                else
                {
                    _shopInventory.AddItem(pItem);
                }
            }
        }
    }

    public CharacterStats Health { get { return _health; } set { _health = value; } }
    public CharacterStats Mana { get { return _mana; } set { _mana = value; } }
    public CharacterStats Armor { get { return _armor; } set { _armor = value; } }
    public CharacterStats Strength { get { return _strength; } set { _strength = value; } }
    public CharacterStats Intelligence { get { return _intelligence; } set { _intelligence = value; } }
}
