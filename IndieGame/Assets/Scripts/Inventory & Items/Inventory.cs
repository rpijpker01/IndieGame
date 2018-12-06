using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    [SerializeField] private List<Item> _startingItems;

    private List<ItemSlot> _listOfConsumablesInInventory = new List<ItemSlot>();

    private Transform _itemsParent;
    private ItemSlot[] _itemSlots;

    private void Awake()
    {
        _itemsParent = transform.GetChild(0).GetComponent<Transform>();
        _itemSlots = _itemsParent.GetComponentsInChildren<ItemSlot>();

        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            _itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            _itemSlots[i].OnEndDragEvent += OnEndDragEvent;
            _itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            _itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            _itemSlots[i].OnDragEvent += OnDragEvent;
            _itemSlots[i].OnDropEvent += OnDropEvent;
        }

        SetStartingItems();
    }

    public bool AddItem(Item pItem)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            Consumable c = _itemSlots[i].Item as Consumable;
            if (_itemSlots[i].Item == null || (c != null && c.ID == pItem.ID && _itemSlots[i].Amount < c.MaxStacks))
            {
                _itemSlots[i].Item = pItem;
                if (_itemSlots[i].Item is Equippable)
                    _itemSlots[i].Amount = 1;
                else
                    _itemSlots[i].Amount++;

                if (_itemSlots[i].Item is Consumable && (c == null || _itemSlots[i].Amount >= c.MaxStacks))
                {
                    _listOfConsumablesInInventory.Add(_itemSlots[i]);
                    print(_listOfConsumablesInInventory.Count);
                }

                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(Item pItem)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i].Item == pItem)
            {
                if (_itemSlots[i].Item is Equippable)
                    _itemSlots[i].Amount = 0;
                else
                    _itemSlots[i].Amount--;

                if (_itemSlots[i].Amount == 0)
                {
                    if (_itemSlots[i].Item is Consumable && _listOfConsumablesInInventory.Contains(_itemSlots[i]))
                        _listOfConsumablesInInventory.Remove(_itemSlots[i]);

                    _itemSlots[i].Item = null;
                }
                return true;
            }
        }

        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i].Item == null)
            {
                return false;
            }
        }

        return true;
    }

    private void SetStartingItems()
    {
        int i = 0;

        if (_startingItems != null)
        {
            for (; i < _startingItems.Count && i < _itemSlots.Length; i++)
            {
                _itemSlots[i].Item = _startingItems[i].GetCopy();
                _itemSlots[i].Amount = 1;

                if (_startingItems[i] is Consumable)
                {
                    Consumable c = _startingItems[i] as Consumable;
                    _itemSlots[i].Amount = c.StartCount;
                    _listOfConsumablesInInventory.Add(_itemSlots[i]);
                }
            }
        }

        for (; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Item = null;
            _itemSlots[i].Amount = 0;
        }
    }

    public ItemSlot[] SlotsInInventory
    {
        get
        {
            return _itemSlots;
        }
    }

    public List<ItemSlot> ConsumablesInInventory
    {
        get { return _listOfConsumablesInInventory; }
        set { _listOfConsumablesInInventory = value; }
    }
}