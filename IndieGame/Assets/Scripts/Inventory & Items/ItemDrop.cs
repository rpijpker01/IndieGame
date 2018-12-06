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

    public void Init(Item pItem, Transform pPos, bool pAddForce = true)
    {
        _playerTransform = GameController.player.GetComponent<Transform>();
        _itemTooltip = GameObject.Find("InGameUICanvas").transform.GetChild(4).GetComponent<ItemTooltip>();
        _playerInventory = GameObject.Find("InGameUICanvas").transform.GetChild(2).transform.GetChild(1).GetComponent<Inventory>();
        _itemFlipAnimation = GetComponent<Animation>();
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();

        //Ignore collision with player
        GameObject itemGo = Instantiate(pItem.PrefabWhenDropped, this.transform);
        _item = pItem;
        itemGo.transform.tag = "LootDrop";
        itemGo.layer = 10;
        GetComponent<HighlightGameobject>().enabled = false;
        GetComponent<HighlightGameobject>().enabled = true;

        GameObject _itemName = Resources.Load<GameObject>("DroppedItemDisplayParent");
        GameObject go = Instantiate(_itemName, Camera.main.WorldToScreenPoint(this.transform.position), Quaternion.identity);
        _textScript = go.GetComponent<FollowItemPosition>();
        _textScript.Item = itemGo.transform;
        _textScript.ItemName = pItem.Name;
        _textScript.Init();

        if (pAddForce)
            _rb.AddForce(Vector3.up * Random.Range(0, 5) + pPos.right * Random.Range(-2, 2) + pPos.forward * Random.Range(2, 6), ForceMode.Impulse);
        else
            _itemFlipAnimation.Play();

        GameController.OnMouseOverGameObjectEvent += Highlight;
        GameController.OnMouseAwayFromGameObject += Shade;
        GameController.OnMouseLeftClickGameObject += PickUp;
        GameController.OnCtrlKeyHoldEvent += ShowTooltip;
        GameController.OnCrtlKeyUpEvent += HideTooltip;
    }

    public void PickUp(GameObject go)
    {
        if (this.gameObject != go) return;

        if (!_playerInventory.IsFull())
        {
            Item itemCopy = _item;
            _playerInventory.AddItem(itemCopy);
            GameController.errorMessage.AddMessage(string.Format("Picked up {0}", itemCopy.Name, Color.white));

            if (OlChapBehaviour.GetQuestProgression() == 2)
            {
                OlChapBehaviour.ContinuePorgression();
                ObjectiveText.SetObjectiveText("- Go back to the old man");
            }

            Destroy(this.gameObject);
            Destroy(_textScript.gameObject);
        }
        else
        {
            transform.parent.position = this.transform.position;
            _itemFlipAnimation.Play();
            GameController.errorMessage.AddMessage("Inventory is full!");
        }
    }

    public void ShowTooltip(GameObject go)
    {
        if (go.GetComponent<ItemDrop>() == null)
            _itemTooltip.HideTooltip();

        if (this.gameObject != go) return;

        _itemTooltip.ShowTooltip(_item);
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
        GameController.OnCtrlKeyHoldEvent -= ShowTooltip;
        GameController.OnCrtlKeyUpEvent -= HideTooltip;
    }

    public Item Item { get { return _item; } }
}