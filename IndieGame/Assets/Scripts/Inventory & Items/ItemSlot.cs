using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    protected Image _image;
    protected Item _item;

    public event Action<Item> OnRightClickEvent;

    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
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
                _image.sprite = _item.Icon;
                _image.enabled = true;
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
                print(Item.Name);
                OnRightClickEvent(Item);
            }
        }
    }
}
