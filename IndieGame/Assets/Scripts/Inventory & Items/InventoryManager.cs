using UnityEngine;
using UnityEngine.UI;
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

    private Image _draggedItem;
    private ItemSlot _draggedSlot;

    private ItemTooltip _itemTooltip;
    private StatTooltip _statTooltip;

    private ShopInventory _shopInventory;

    private void Awake()
    {
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        _equipmentPanel = GameObject.Find("EquipmentPanel").GetComponent<EquipmentPanel>();

        _shopInventory = GameObject.Find("ShopPanel").GetComponent<ShopInventory>();
        _playerCoins = GameObject.Find("Coins").GetComponent<PlayerCoins>();

        _itemTooltip = GameObject.Find("ItemTooltip").GetComponent<ItemTooltip>();
        _statTooltip = GameObject.Find("StatTooltip").GetComponent<StatTooltip>();
        _draggedItem = GameObject.Find("DraggedItem").GetComponent<Image>();
        _draggedItem.enabled = false;

        _statsPanel = GameObject.Find("Stats").GetComponent<StatPanel>();
        _statsPanel.SetStats(_health, _mana, _armor, _strength, _intelligence);
        _statsPanel.UpdateStatValues();

        //Setup Events:
        //Right Click
        _inventory.OnRightClickEvent += Equip;
        _inventory.OnRightClickEvent += Sell;
        _inventory.OnRightClickEvent += HideTooltip;
        _equipmentPanel.OnRightClickEvent += Unequip;
        _shopInventory.OnRightClickEvent += Buy;
        _shopInventory.OnRightClickEvent += HideTooltip;
        //Pointer Enter
        _inventory.OnPointerEnterEvent += ShowTooltip;
        _equipmentPanel.OnPointerEnterEvent += ShowTooltip;
        _shopInventory.OnPointerEnterEvent += ShowTooltip;
        //Pointer Exit
        _inventory.OnPointerExitEvent += HideTooltip;
        _equipmentPanel.OnPointerExitEvent += HideTooltip;
        _shopInventory.OnPointerExitEvent += HideTooltip;
        //Begin Drag
        _inventory.OnBeginDragEvent += BeginDrag;
        _inventory.OnBeginDragEvent += HideTooltip;
        _equipmentPanel.OnBeginDragEvent += BeginDrag;
        //End Drag
        _inventory.OnEndDragEvent += EndDrag;
        _equipmentPanel.OnEndDragEvent += EndDrag;
        //Drag
        _inventory.OnDragEvent += Drag;
        _equipmentPanel.OnDragEvent += Drag;
        //Drop
        _inventory.OnDropEvent += Drop;
        _inventory.OnDropEvent += DragToBuy;
        _inventory.OnDropEvent += ShowTooltip;
        _equipmentPanel.OnDropEvent += Drop;
        _equipmentPanel.OnDropEvent += ShowTooltip;
        _shopInventory.OnDropEvent += Drop;
        _shopInventory.OnDropEvent += DragToSell;
        _shopInventory.OnDropEvent += ShowTooltip;
    }

    private void DragToSell(ItemSlot pItemSlot)
    {
        if (!(pItemSlot is ShopItemSlot)) return;

        Item i = pItemSlot.Item as Item;
        if (i != null)
        {
            DragToSell(i);
        }
    }

    private void DragToBuy(ItemSlot pItemSlot)
    {
        if (!(pItemSlot is ShopItemSlot)) return;

        Item i = pItemSlot.Item as Item;
        if (i != null)
        {
            DragToBuy(i);
        }
    }

    private void Sell(ItemSlot pItemSlot)
    {
        Item i = pItemSlot.Item as Item;
        if (i != null)
        {
            Sell(i);
        }
    }

    private void Buy(ItemSlot pItemSlot)
    {
        Item i = pItemSlot.Item as Item;
        if (i != null)
        {
            Buy(i);
        }
    }

    private void Equip(ItemSlot pItemSlot)
    {
        if (ShopKeeper.playerIsInShop) return;

        Equippable eq = pItemSlot.Item as Equippable;
        if (eq != null)
        {
            Equip(eq);
        }
    }

    private void Unequip(ItemSlot pItemSlot)
    {
        Equippable eq = pItemSlot.Item as Equippable;
        if (eq != null)
        {
            Unequip(eq);
        }
    }

    private void ShowTooltip(ItemSlot pItemSlot)
    {
        Equippable eq = pItemSlot.Item as Equippable;
        if (eq != null)
        {
            _itemTooltip.ShowTooltip(eq);
        }
    }

    private void HideTooltip(ItemSlot pItemSlot)
    {
        _itemTooltip.HideTooltip();
    }

    private void BeginDrag(ItemSlot pItemSlot)
    {
        if (pItemSlot.Item != null)
        {
            _draggedSlot = pItemSlot;
            _draggedItem.sprite = pItemSlot.Item.IconInInv;
            _draggedItem.color = new Color(_draggedItem.color.r, _draggedItem.color.g, _draggedItem.color.b, 0.5f);
            _draggedItem.transform.position = Input.mousePosition;
            _draggedItem.enabled = true;
        }
    }

    private void EndDrag(ItemSlot pItemSlot)
    {
        _draggedSlot = null;
        _draggedItem.enabled = false;
    }

    private void Drag(ItemSlot pItemSlot)
    {
        if (!_draggedItem.enabled) return;
        _draggedItem.transform.position = Input.mousePosition;
    }

    private void Drop(ItemSlot pDropItemSlot)
    {
        if (_draggedSlot == null || pDropItemSlot == null) return;

        if (pDropItemSlot.CanReceiveItem(_draggedSlot.Item) && _draggedSlot.CanReceiveItem(pDropItemSlot.Item))
        {
            Equippable dragItem = _draggedSlot.Item as Equippable;
            Equippable dropItem = pDropItemSlot.Item as Equippable;

            if (_draggedSlot is EquipmentSlot)
            {
                if (dragItem != null) dragItem.Unequip(this);
                if (dropItem != null) dropItem.Equip(this);
            }

            if (pDropItemSlot is EquipmentSlot)
            {
                if (dropItem != null) dropItem.Unequip(this);
                if (dragItem != null) dragItem.Equip(this);
            }

            _statsPanel.UpdateStatValues();

            Item di = _draggedSlot.Item;
            _draggedSlot.Item = pDropItemSlot.Item;
            pDropItemSlot.Item = di;
        }
        else
        {
            if (pDropItemSlot is EquipmentSlot)
            {
                GameController.errorMessage.DisplayMessage("Item cannot be equipped in this slot!");
            }
            else if (pDropItemSlot is ShopItemSlot)
            {
                GameController.errorMessage.DisplayMessage("Shop is full");
            }
            else
            {
                GameController.errorMessage.DisplayMessage("Inventory is full");
            }
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

    private void DragToSell(Item pItem)
    {
        if (!ShopKeeper.playerIsInShop) return;
        if (_shopInventory == null)
            _shopInventory = GameObject.Find("ShopInventory").GetComponent<ShopInventory>();

        Equippable e = pItem as Equippable;
        if (e != null)
        {
            GameController.errorMessage.DisplayMessage(e.Name + "\nhas been sold!");
            _playerCoins.AddCoins(e.Value);
        }
    }

    private void DragToBuy(Item pItem)
    {
        if (!ShopKeeper.playerIsInShop) return;
        if (_shopInventory == null)
            _shopInventory = GameObject.Find("ShopInventory").GetComponent<ShopInventory>();

        Equippable e = pItem as Equippable;
        if (e != null)
        {
            if (_playerCoins.TakeCoins(Mathf.RoundToInt(e.Value * 1.25f)))
            {
                GameController.errorMessage.DisplayMessage(e.Name + "\nhas been purchased!");
            }
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
