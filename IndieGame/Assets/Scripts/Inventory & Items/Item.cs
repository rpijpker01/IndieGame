using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] protected string _name;
    [SerializeField] protected Sprite _iconInInventory;
    [SerializeField] protected Sprite _iconWhenEquipped;

    public string Name { get { return _name; } set { _name = value; } }
    public Sprite IconInInv { get { return _iconInInventory; } set { _iconInInventory = value; } }
    public Sprite IconWhenEquipped { get { return _iconWhenEquipped; } set { _iconWhenEquipped = value; } }
}