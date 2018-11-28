using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    [SerializeField] private List<Item> _startingItems;

    private Transform _itemsParent;
    private ShopItemSlot[] _itemSlots;

    private void Awake()
    {
        _itemsParent = transform.GetChild(0).GetComponent<Transform>();
        _itemSlots = _itemsParent.GetComponentsInChildren<ShopItemSlot>();

        for (int i = 0; i < _itemSlots.Length; i++)
        {
            //Call this event whenever an item from the inventory is right-clicked
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
            if (_itemSlots[i].Item == null)
            {
                _itemSlots[i].Item = pItem;
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
                _itemSlots[i].Item = null;
                return true;
            }
        }

        return false;
    }

    public Item RemoveItem(string pID)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            Item item = _itemSlots[i].Item;
            if (item != null && item.ID == pID)
            {
                _itemSlots[i].Item = null;
                return item;
            }
        }

        return null;
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
                _itemSlots[i].Item = Instantiate(_startingItems[i]);
        }

        for (; i < _itemSlots.Length; i++)
            _itemSlots[i].Item = null;
    }
}
