﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FollowItemPosition : MonoBehaviour
{
    [SerializeField] private Text _textObject;
    [SerializeField] private Image _background;

    private Transform _item;
    private ItemDrop _itemScript;
    private string _itemName;
    private int _oldIndex;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_item.transform.position) + Vector3.up * 50;
    }

    public void Init()
    {
        GameObject go = GameObject.Find("ContainerForItemNameDisplays");
        transform.SetParent(go.transform);
        _textObject.text = _itemName;
        _itemScript = _item.GetComponent<ItemDrop>();

        _background.enabled = false;
        _textObject.enabled = false;
    }

    public void Highlight()
    {
        _background.enabled = true;
        _textObject.enabled = true;
    }

    public void Shade()
    {
        _background.enabled = false;
        _textObject.enabled = false;
    }

    public string ItemName
    {
        get { return _itemName; }
        set { _itemName = value; }
    }

    public Transform Item
    {
        get { return _item; }
        set { _item = value; }
    }
}