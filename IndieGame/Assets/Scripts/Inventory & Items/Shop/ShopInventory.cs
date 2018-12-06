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
            GameController.OnBackToHubEvent += RollNewItems;
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
                _itemSlots[i].Amount++;
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
                _itemSlots[i].Amount--;
                if (_itemSlots[i].Amount == 0)
                    _itemSlots[i].Item = null;
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

        GameController.errorMessage.AddMessage("Shop is full!");
        return true;
    }

    private void RollNewItems(List<Item> pList)
    {
        for (int j = 0; j < _itemSlots.Length; j++)
        {
            _itemSlots[j].Item = null;
            _itemSlots[j].Amount = 0;
        }

        int i = 0;

        for (; i < _itemSlots.Length && i < 10; i++)
        {
            int rnd = UnityEngine.Random.Range(0, pList.Count);

            _itemSlots[i].Item = pList.ToArray()[rnd].GetCopy();
            _itemSlots[i].Item.IsInShop = true;
            _itemSlots[i].Amount = 1;

            if (pList.ToArray()[rnd] is Consumable)
            {
                Consumable c = pList.ToArray()[rnd] as Consumable;
                _itemSlots[i].Amount = 20;
            }
        }

        for (; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Item = null;
            _itemSlots[i].Amount = 0;
        }
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
                }
            }
        }

        for (; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Item = null;
            _itemSlots[i].Amount = 0;
        }
    }
}
