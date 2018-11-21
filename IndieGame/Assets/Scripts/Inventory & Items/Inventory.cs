using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action<Item> OnInventoryItemRightClickEvent;

    public List<Item> _items;
    private Transform _itemsParent;
    private ItemSlot[] _itemSlots;

    private void Awake()
    {
        _itemsParent = transform.GetChild(0).GetComponent<Transform>();
        _itemSlots = _itemsParent.GetComponentsInChildren<ItemSlot>();

        for (int i = 0; i < _itemSlots.Length; i++)
        {
            //Call this event whenever an item from the inventory is right-clicked
            _itemSlots[i].OnRightClickEvent += OnInventoryItemRightClickEvent;
        }

        Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            transform.parent.GetComponent<Canvas>().enabled = !transform.parent.GetComponent<Canvas>().isActiveAndEnabled;
        if (Input.GetKeyDown(KeyCode.G))
        {
            Refresh();
        }
    }

    public bool AddItem(Item pItem)
    {
        if (IsFull()) return false;

        _items.Add(pItem);
        Refresh();

        return true;
    }

    public bool RemoveItem(Item pItem)
    {
        if (_items.Remove(pItem))
        {
            Refresh();
            return true;
        }

        return false;
    }

    public bool IsFull()
    {
        return _items.Count >= _itemSlots.Length;
    }

    private void Refresh()
    {
        int i = 0;

        if (_items != null)
        {
            for (; i < _items.Count && i < _itemSlots.Length; i++)
                _itemSlots[i].Item = _items[i];
        }

        for (; i < _itemSlots.Length; i++)
            _itemSlots[i].Item = null;
    }
}