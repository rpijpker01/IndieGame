using UnityEngine;
using UnityEngine.UI;

public class FollowItemPosition : MonoBehaviour
{
    [SerializeField] private Text _textObject;
    private Transform _item;
    private string _itemName;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_item.transform.position) + Vector3.up * 50;
    }

    public void Init()
    {
        GameObject go = GameObject.Find("InGameUICanvas");
        transform.SetParent(go.transform);
        transform.SetAsFirstSibling();
        _textObject.text = _itemName;
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
