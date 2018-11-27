using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour
{
    private Transform _equipmentPanelParent;
    private EquipmentSlot[] _equipmentSlots;

    public event Action<Item> OnEquipmentPanelItemRightClickEvent;

    private void Awake()
    {
        //Assign the needed variables in Awake
        _equipmentPanelParent = transform.GetChild(0).GetComponent<Transform>();
        _equipmentSlots = _equipmentPanelParent.GetComponentsInChildren<EquipmentSlot>();

        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            _equipmentSlots[i].OnRightClickEvent += OnEquipmentPanelItemRightClickEvent;
            _equipmentSlots[i].GetComponent<Image>().enabled = false;
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
}
