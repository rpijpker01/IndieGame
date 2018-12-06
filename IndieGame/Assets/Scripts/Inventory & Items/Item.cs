using UnityEngine;

public enum DropsFrom
{
    Enemies,
    Chests,
    Everywhere
}

public class Item : ScriptableObject
{
    [SerializeField] protected string _id;
    [SerializeField] protected string _name;
    [SerializeField] protected Sprite _iconInInventory;
    [SerializeField] protected Sprite _iconWhenEquipped;
    [SerializeField] protected GameObject _prefabWhenDropped;
    [Space]
    [Tooltip("Items in the shop are 25% more expensive")]
    [SerializeField] protected Vector2Int _priceRange;
    protected int _itemValue;
    [Space]
    [Range(0, 100)]
    [SerializeField] protected float _dropChancePercent;
    [SerializeField] DropsFrom _dropsFrom;

    private bool _isInShop;

    public virtual Item GetCopy()
    {
        return this;
    }

    public string ID { get { return _id; } }
    public string Name { get { return _name; } set { _name = value; } }
    public bool IsInShop { get { return _isInShop; } set { _isInShop = value; } }
    public Sprite IconInInv { get { return _iconInInventory; } set { _iconInInventory = value; } }
    public Sprite IconWhenEquipped { get { return _iconWhenEquipped; } set { _iconWhenEquipped = value; } }
    public GameObject PrefabWhenDropped { get { return _prefabWhenDropped; } }
    public DropsFrom DropsFrom { get { return _dropsFrom; } }
    public float DropChance { get { return _dropChancePercent; } }
    public int Value
    {
        get
        {
            if (_isInShop)
            {
                return Mathf.RoundToInt(_itemValue * 1.25f);
            }
            else
                return _itemValue;
        }
    }
}