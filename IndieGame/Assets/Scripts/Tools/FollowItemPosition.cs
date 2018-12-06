using UnityEngine;
using UnityEngine.UI;

public class FollowItemPosition : MonoBehaviour
{
    [SerializeField] private Text _textObject;
    [SerializeField] private Image _background;

    private Transform _item;
    private ItemDrop _itemScript;
    private string _itemName;
    private int _oldIndex;
    private bool _showAll;

    private void Update()
    {
        if (_item != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(_item.transform.position) + Vector3.up * 50;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Init()
    {
        GameObject go = GameObject.Find("ContainerForItemNameDisplays");
        transform.SetParent(go.transform);
        _textObject.text = _itemName;
        _itemScript = _item.GetComponent<ItemDrop>();

        GameController.OnAltKeyDownEvent += ShowAll;
        GameController.OnAltKeyUpEvent += HideAll;

        _background.enabled = false;
        _textObject.enabled = false;
    }

    private void OnDestroy()
    {
        GameController.OnAltKeyDownEvent -= ShowAll;
        GameController.OnAltKeyUpEvent -= HideAll;
    }

    public void Highlight()
    {
        _background.enabled = true;
        _textObject.enabled = true;
    }

    public void Shade()
    {
        if (_showAll) return;
        _background.enabled = false;
        _textObject.enabled = false;
    }

    private void ShowAll()
    {
        _showAll = true;
        _background.enabled = true;
        _textObject.enabled = true;
    }

    private void HideAll()
    {
        _showAll = false;
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
