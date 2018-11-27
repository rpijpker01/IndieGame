using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrop : MonoBehaviour
{
    private Item _item;
    private Inventory _inventory;
    private SpriteRenderer _sr;
    private Rigidbody _rb;
    private Transform _playerTransform;

    public void Init(Item pItem)
    {
        _playerTransform = GameController.player.GetComponent<Transform>();
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody>();

        _item = pItem;
        _sr.sprite = pItem.IconInInv;

        GameObject _itemName = Resources.Load<GameObject>("DroppedItemDisplayParent");
        GameObject go = Instantiate(_itemName, Camera.main.WorldToScreenPoint(this.transform.position), Quaternion.identity);
        FollowItemPosition textScript = go.GetComponent<FollowItemPosition>();
        textScript.Item = this.transform;
        textScript.ItemName = pItem.Name;
        textScript.Init();

        _rb.AddForce(transform.up * Random.Range(-10, 0) + transform.right * Random.Range(-5, 5) + transform.forward * Random.Range(-5, 5), ForceMode.Impulse);
    }
}