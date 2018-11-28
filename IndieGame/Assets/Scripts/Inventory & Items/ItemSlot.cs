using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    protected Image _image;
    protected Item _item;

    protected Color _normalCol = Color.white;
    protected Color _noAlphaCol = new Color(1, 1, 1, 0);

    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;


    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
    }

    public virtual Item Item
    {
        get { return _item; }
        set
        {
            _item = value;

            if (_item == null)
            {
                _image.color = _noAlphaCol;
            }
            else
            {
                _image.sprite = _item.IconInInv;
                _image.color = _normalCol;
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

    public virtual bool CanReceiveItem(Item pItem)
    {
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Using the unity interface, check if an item is right-clicked
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (OnRightClickEvent != null)
                OnRightClickEvent(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterEvent != null)
            OnPointerEnterEvent(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitEvent != null)
            OnPointerExitEvent(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (OnBeginDragEvent != null)
            OnBeginDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragEvent != null)
            OnEndDragEvent(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (OnDropEvent != null)
            OnDropEvent(this);
    }
}
