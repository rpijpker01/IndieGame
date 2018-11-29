using UnityEngine;
using F.CharacterStats;

public enum EquipmentType
{
    Helm,
    Chest,
    Gloves,
    Gloves1,
    Pants,
    Boots,
    Weapon,
    OffHand
}

[CreateAssetMenu]
public class Equippable : Item
{
    [SerializeField] private EquipmentType _equipmentType;

    [Header("Flat Stats:\t\tMin\t\tMax")]
    [SerializeField] private Vector2 _healthValue;
    [SerializeField] private Vector2 _manaValue;
    [SerializeField] private Vector2 _armorValue;
    [SerializeField] private Vector2 _strengthValue;
    [SerializeField] private Vector2 _intelligenceValue;
    [Space]
    [Header("Percent Stats:\t\tMin\t\tMax")]
    [SerializeField] private Vector2 _healthValuePercent;
    [SerializeField] private Vector2 _manaValuePercent;
    [SerializeField] private Vector2 _armorValuePercent;
    [SerializeField] private Vector2 _strengthValuePercent;
    [SerializeField] private Vector2 _intelligenceValuePercent;
    [Space]
    [SerializeField] private int _itemValue;
    [Space]
    [SerializeField] [Range(0, 100)] private float _dropChancePercent;
    [Header("Drops in:")]
    [SerializeField] private bool _zoneOne;
    [SerializeField] private bool _zoneTwo;
    [SerializeField] private bool _zoneThree;
    [SerializeField] private bool _zoneFour;
    [SerializeField] private bool _zoneFive;
    [SerializeField] private bool _allZones;

    private float _health;
    private float _healthPercent;
    private float _mana;
    private float _manaPercent;
    private float _armor;
    private float _armorPercent;
    private float _strength;
    private float _strengthPercent;
    private float _intelligence;
    private float _intelligencePercent;

    private void Awake()
    {
        _health = (int)Random.Range(_healthValue.x, _healthValue.y);
        _healthPercent = (int)Random.Range(_healthValuePercent.x, _healthValuePercent.y);

        _mana = (int)Random.Range(_manaValue.x, _manaValue.y);
        _manaPercent = (int)Random.Range(_manaValuePercent.x, _manaValuePercent.y);

        _armor = (int)Random.Range(_armorValue.x, _armorValue.y);
        _armorPercent = (int)Random.Range(_armorValuePercent.x, _armorValuePercent.y);

        _strength = (int)Random.Range(_strengthValue.x, _strengthValue.y);
        _strengthPercent = (int)Random.Range(_strengthValuePercent.x, _strengthValuePercent.y);

        _intelligence = (int)Random.Range(_intelligenceValue.x, _intelligenceValue.y);
        _intelligencePercent = (int)Random.Range(_intelligenceValuePercent.x, _intelligenceValuePercent.y);
    }

    public void Equip(InventoryManager pInv)
    {
        //Health & %Health
        if (_healthValue.x != 0 && _healthValue.x <= _healthValue.y)
        {
            pInv.Health.AddModifier(new StatModifier(_health, StatModifierType.Flat, this));
        }
        if (_healthValuePercent.x != 0 && _healthValuePercent.x <= _healthValuePercent.y)
        {
            pInv.Health.AddModifier(new StatModifier(_healthPercent, StatModifierType.PercentAdd, this));
        }

        //Mana & %Mana
        if (_manaValue.x != 0 && _manaValue.x <= _manaValue.y)
        {
            pInv.Mana.AddModifier(new StatModifier(_mana, StatModifierType.Flat, this));
        }
        if (_manaValuePercent.x != 0 && _manaValuePercent.x <= _manaValuePercent.y)
        {
            pInv.Mana.AddModifier(new StatModifier(_manaPercent, StatModifierType.PercentAdd, this));
        }

        //Armor & %Armor
        if (_armorValue.x != 0 && _armorValue.x <= _armorValue.y)
        {
            pInv.Armor.AddModifier(new StatModifier(_armor, StatModifierType.Flat, this));
        }
        if (_armorValuePercent.x != 0 && _armorValuePercent.x <= _armorValuePercent.y)
        {
            pInv.Armor.AddModifier(new StatModifier(_armorPercent, StatModifierType.PercentAdd, this));
        }

        //StrengthMana & %Strength
        if (_strengthValue.x != 0 && _strengthValue.x <= _strengthValue.y)
        {
            pInv.Strength.AddModifier(new StatModifier(_strength, StatModifierType.Flat, this));
        }
        if (_strengthValuePercent.x != 0 && _strengthValuePercent.x <= _strengthValuePercent.y)
        {
            pInv.Strength.AddModifier(new StatModifier(_strengthPercent, StatModifierType.PercentAdd, this));
        }

        //Intelligence & %Intelligence
        if (_intelligenceValue.x != 0 && _intelligenceValue.x <= _intelligenceValue.y)
        {
            pInv.Intelligence.AddModifier(new StatModifier(_intelligence, StatModifierType.Flat, this));
        }
        if (_intelligenceValuePercent.x != 0 && _intelligenceValuePercent.x <= _intelligenceValuePercent.y)
        {
            pInv.Intelligence.AddModifier(new StatModifier(_intelligencePercent, StatModifierType.PercentAdd, this));
        }
    }

    public void Unequip(InventoryManager pInv)
    {
        pInv.Health.RemoveAllModifiersFromSource(this);
        pInv.Mana.RemoveAllModifiersFromSource(this);
        pInv.Armor.RemoveAllModifiersFromSource(this);
        pInv.Strength.RemoveAllModifiersFromSource(this);
        pInv.Intelligence.RemoveAllModifiersFromSource(this);
    }

    public EquipmentType ItemType { get { return _equipmentType; } set { _equipmentType = value; } }
    public float Health { get { return _health; } }
    public float Mana { get { return _mana; } }
    public float Armor { get { return _armor; } }
    public float Strength { get { return _strength; } }
    public float Intelligence { get { return _intelligence; } }

    public float HealthPercent { get { return _healthPercent; } }
    public float Manapercent { get { return _manaPercent; } }
    public float ArmorPercent { get { return _armorPercent; } }
    public float StrengthPercent { get { return _strengthPercent; } }
    public float IntelligencePercent { get { return _intelligencePercent; } }

    public bool ZoneOne { get { return _zoneOne; } }
    public bool ZoneTwo { get { return _zoneTwo; } }
    public bool ZoneThree { get { return _zoneThree; } }
    public bool ZoneFour { get { return _zoneFour; } }
    public bool ZoneFive { get { return _zoneFive; } }
    public bool AllZone { get { return _allZones; } }

    public float DropChance { get { return _dropChancePercent; } }
    public int Value { get { return _itemValue; } }
}