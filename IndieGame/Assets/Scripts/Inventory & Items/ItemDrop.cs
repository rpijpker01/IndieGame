using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrop : MonoBehaviour
{
    private Item _item;
    private SpriteRenderer _sr;
    private Rigidbody _rb;
    private Transform _playerTransform;
    private FollowItemPosition _textScript;

    private ItemTooltip _itemTooltip;
    private Inventory _playerInventory;
    private Animation _itemFlipAnimation;

    public void Init(Item pItem)
    {
        _playerTransform = GameController.player.GetComponent<Transform>();
        _itemTooltip = GameObject.Find("InGameUICanvas").transform.GetChild(4).GetComponent<ItemTooltip>();
        _playerInventory = GameObject.Find("InGameUICanvas").transform.GetChild(2).transform.GetChild(1).GetComponent<Inventory>();
        _itemFlipAnimation = GetComponent<Animation>();

        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody>();

        //Ignore collision with player
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), _playerTransform.GetComponent<Collider>());

        _item = Instantiate(pItem);
        _sr.sprite = pItem.IconInInv;

        GameObject _itemName = Resources.Load<GameObject>("DroppedItemDisplayParent");
        GameObject go = Instantiate(_itemName, Camera.main.WorldToScreenPoint(this.transform.position), Quaternion.identity);
        _textScript = go.GetComponent<FollowItemPosition>();
        _textScript.Item = this.transform;
        _textScript.ItemName = pItem.Name;
        _textScript.Init();

        _rb.AddForce(transform.up * Random.Range(-10, 0) + transform.right * Random.Range(-5, 5) + transform.forward * Random.Range(-5, 5), ForceMode.Impulse);

        GameController.OnMouseOverGameObjectEvent += Highlight;
        GameController.OnMouseAwayFromGameObject += Shade;
        GameController.OnMouseLeftClickGameObject += PickUp;
        GameController.OnCtrlButtonHoldEvent += ShowTooltip;
        GameController.OnCrtlButtonLetgoEvent += HideTooltip;
    }

    private void Update()
    {
        Vector3 distoToPlayer = _playerTransform.position - this.transform.position;
    }

    public void PickUp(GameObject go)
    {
        if (this.gameObject != go) return;

        if (!_playerInventory.IsFull())
        {
            _playerInventory.AddItem(_item);
            GameController.errorMessage.DisplayMessage(string.Format("Picked up\n{0}", _item.Name));
            Destroy(this.gameObject);
            Destroy(_textScript.gameObject);
        }
        else
        {
            transform.parent.position = this.transform.position;
            _itemFlipAnimation.Play();
            GameController.errorMessage.DisplayMessage("Inventory is full!");
        }
    }

    public void ShowTooltip(GameObject go)
    {
        if (this.gameObject != go) return;

        if (_item is Equippable)
            _itemTooltip.ShowTooltip((Equippable)_item);
    }

    public void HideTooltip(GameObject go)
    {
        if (this.gameObject == go) return;

        _itemTooltip.HideTooltip();
    }

    public void Highlight(GameObject go)
    {
        if (this.gameObject != go) return;

        _textScript.Highlight();
    }

    public void Shade(GameObject go)
    {
        if (this.gameObject == go) return;

        _textScript.Shade();
    }

    private void OnDestroy()
    {
        GameController.OnMouseOverGameObjectEvent -= Highlight;
        GameController.OnMouseAwayFromGameObject -= Shade;
        GameController.OnMouseLeftClickGameObject -= PickUp;
        GameController.OnCtrlButtonHoldEvent -= ShowTooltip;
        GameController.OnCrtlButtonLetgoEvent -= HideTooltip;
    }

    public Item Item { get { return _item; } }
}