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
    private EquipmentType _equipmentType;

    [Header("Flat Stats:\t\tMin\t\tMax")]
    [Tooltip("Increases the Player's maximum Health (allowing them to take more damage)")]
    [SerializeField] private Vector2 _healthValue;
    [Tooltip("Increases the Player's maximum Mana (allowing them to cast more abilities)")]
    [SerializeField] private Vector2 _manaValue;
    [Tooltip("Increases the Player's maximum Armor (for every 10 Armor the Player mitigates 1 damage taken from enemies)")]
    [SerializeField] private Vector2 _armorValue;
    [Tooltip("Increases the Player's Strength (for every 10 Strength the Player deals 1 additional damage with physical attacks)")]
    [SerializeField] private Vector2 _strengthValue;
    [Tooltip("Increases the Player's Intelligence (for every 10 Ingellignce the Player deals 1 additional damage with magic attacks)")]
    [SerializeField] private Vector2 _intelligenceValue;
    [Space]
    [Header("Percent Stats:\t\tMin\t\tMax")]
    [Tooltip("Increases the Player's maximum Health (allowing them to take more damage)")]
    [SerializeField] private Vector2 _healthValuePercent;
    [Tooltip("Increases the Player's maximum Mana (allowing them to cast more abilities)")]
    [SerializeField] private Vector2 _manaValuePercent;
    [Tooltip("Increases the Player's maximum Armor (for every 10 Armor the Player mitigates 1 damage taken from enemies)")]
    [SerializeField] private Vector2 _armorValuePercent;
    [Tooltip("Increases the Player's Strength (for every 10 Strength the Player deals 1 additional damage with physical attacks)")]
    [SerializeField] private Vector2 _strengthValuePercent;
    [Tooltip("Increases the Player's Intelligence (for every 10 Ingellignce the Player deals 1 additional damage with magic attacks)")]
    [SerializeField] private Vector2 _intelligenceValuePercent;

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

        _itemValue = Random.Range(_priceRange.x, _priceRange.y);

        switch (this.name.ToLower().ToCharArray()[0])
        {
            case 'h':
                _equipmentType = EquipmentType.Helm;
                break;
            case 'c':
                _equipmentType = EquipmentType.Chest;
                break;
            case 'g':
                _equipmentType = EquipmentType.Gloves;
                break;
            case 'p':
                _equipmentType = EquipmentType.Pants;
                break;
            case 'b':
                _equipmentType = EquipmentType.Boots;
                break;
            case 'w':
                _equipmentType = EquipmentType.Weapon;
                break;
            case 'o':
                _equipmentType = EquipmentType.OffHand;
                break;
        }
    }

    public override Item GetCopy()
    {
        return Instantiate(this);
    }

    public void Equip(InventoryManager pInv)
    {
        //Health & %Health
        if (_health != 0)
        {
            pInv.Health.AddModifier(new StatModifier(_health, StatModifierType.Flat, this));
        }
        if (_healthPercent != 0)
        {
            pInv.Health.AddModifier(new StatModifier(_healthPercent, StatModifierType.PercentAdd, this));
        }

        //Mana & %Mana
        if (_mana != 0)
        {
            pInv.Mana.AddModifier(new StatModifier(_mana, StatModifierType.Flat, this));
        }
        if (_manaPercent != 0)
        {
            pInv.Mana.AddModifier(new StatModifier(_manaPercent, StatModifierType.PercentAdd, this));
        }

        //Armor & %Armor
        if (_armor != 0)
        {
            pInv.Armor.AddModifier(new StatModifier(_armor, StatModifierType.Flat, this));
        }
        if (_armorPercent != 0)
        {
            pInv.Armor.AddModifier(new StatModifier(_armorPercent, StatModifierType.PercentAdd, this));
        }

        //StrengthMana & %Strength
        if (_strength != 0)
        {
            pInv.Strength.AddModifier(new StatModifier(_strength, StatModifierType.Flat, this));
        }
        if (_strengthPercent != 0)
        {
            pInv.Strength.AddModifier(new StatModifier(_strengthPercent, StatModifierType.PercentAdd, this));
        }

        //Intelligence & %Intelligence
        if (_intelligence != 0)
        {
            pInv.Intelligence.AddModifier(new StatModifier(_intelligence, StatModifierType.Flat, this));
        }
        if (_intelligencePercent != 0)
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
}