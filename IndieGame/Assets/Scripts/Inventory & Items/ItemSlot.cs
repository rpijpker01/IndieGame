using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected Image _image;
    protected Item _item;

    private ItemTooltip _tooltip;

    public event Action<Item> OnRightClickEvent;

    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
        _tooltip = GameObject.Find("ItemTooltip").GetComponent<ItemTooltip>();
        OnRightClickEvent += _tooltip.HideTooltip;
    }

    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;

            if (_item == null)
            {
                _image.enabled = false;
            }
            else
            {
                _image.sprite = _item.IconInInv;
                _image.enabled = true;
            }

            if (_item is Equippable)
            {
                Equippable e = (Equippable)_item;

                switch (e.name.ToLower().ToCharArray()[0])
                {
                    case 'h':
                    e.ItemType = EquipmentType.Helm;
                    break;
                    case 'c':
                    e.ItemType = EquipmentType.Chest;
                    break;
                    case 'g':
                    e.ItemType = EquipmentType.Gloves;
                    break;
                    case 'p':
                    e.ItemType = EquipmentType.Pants;
                    break;
                    case 'b':
                    e.ItemType = EquipmentType.Boots;
                    break;
                    case 'w':
                    e.ItemType = EquipmentType.Weapon;
                    break;
                    case 'o':
                    e.ItemType = EquipmentType.OffHand;
                    break;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Using the unity interface, check if an item is right-clicked
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null)
            {
                OnRightClickEvent(Item);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item is Equippable)
            _tooltip.ShowTooltip((Equippable)Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.HideTooltip();
    }
}
