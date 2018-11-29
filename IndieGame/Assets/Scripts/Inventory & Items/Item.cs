using UnityEngine;
using UnityEditor;

public class Item : ScriptableObject
{
    [SerializeField] protected string _id;
    [SerializeField] protected string _name;
    [SerializeField] protected Sprite _iconInInventory;
    [SerializeField] protected Sprite _iconWhenEquipped;

    private void Awake()
    {
        string path = AssetDatabase.GetAssetPath(this);
        _id = AssetDatabase.AssetPathToGUID(path);
    }

    public string ID { get { return _id; } }
    public string Name { get { return _name; } set { _name = value; } }
    public Sprite IconInInv { get { return _iconInInventory; } set { _iconInInventory = value; } }
    public Sprite IconWhenEquipped { get { return _iconWhenEquipped; } set { _iconWhenEquipped = value; } }
}