using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour
{
    private Transform _equipmentPanelParent;
    private EquipmentSlot[] _equipmentSlots;

    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Awake()
    {
        //Assign the needed variables in Awake
        _equipmentPanelParent = transform.GetChild(0).GetComponent<Transform>();
        _equipmentSlots = _equipmentPanelParent.GetComponentsInChildren<EquipmentSlot>();

        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            _equipmentSlots[i].OnRightClickEvent += OnRightClickEvent;
            _equipmentSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            _equipmentSlots[i].OnEndDragEvent += OnEndDragEvent;
            _equipmentSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            _equipmentSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            _equipmentSlots[i].OnDragEvent += OnDragEvent;
            _equipmentSlots[i].OnDropEvent += OnDropEvent;
            _equipmentSlots[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public bool AddItem(Equippable pItem, out Equippable pPrevioursItem)
    {
        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            //Use the out parameter to check if there is an item in that equipment slot
            if (_equipmentSlots[i].EquipmentType == pItem.ItemType)
            {
                pPrevioursItem = (Equippable)_equipmentSlots[i].Item;

                if (pItem.ItemType == EquipmentType.Gloves)
                    _equipmentSlots[i + 1].Item = pItem;
                else if (pItem.ItemType == EquipmentType.Gloves1)
                    _equipmentSlots[i - 1].Item = pItem;
                _equipmentSlots[i].Item = pItem;
                return true;
            }
        }

        pPrevioursItem = null;
        return false;
    }

    public bool RemoveItem(Equippable pItem)
    {
        //Loop through the array until the item is found and remove it from that equipment slot
        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            if (_equipmentSlots[i].Item == pItem)
            {
                if (pItem.ItemType == EquipmentType.Gloves)
                    _equipmentSlots[i + 1].Item = null;
                else if (pItem.ItemType == EquipmentType.Gloves1)
                    _equipmentSlots[i - 1].Item = null;

                _equipmentSlots[i].Item = null;
                return true;
            }
        }

        return false;
    }

    public EquipmentSlot[] EquipmentSlots { get { return _equipmentSlots; } }
}
