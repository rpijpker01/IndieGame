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

    [SerializeField] private float _healthValue;
    [SerializeField] private float _manaValue;
    [SerializeField] private float _armorValue;
    [SerializeField] private float _strenghtValue;
    [SerializeField] private float _intelligenceValue;
    [Space]
    [SerializeField] private float _healthValuePercent;
    [SerializeField] private float _manaValuePercent;
    [SerializeField] private float _armorValuePercent;
    [SerializeField] private float _strenghtValuePercent;
    [SerializeField] private float _intelligenceValuePercent;

    public void Equip(InventoryManager pInv)
    {
        if (_healthValue != 0)
            pInv.Health.AddModifier(new StatModifier(_healthValue, StatModifierType.Flat, this));
        if (_healthValuePercent != 0)
            pInv.Health.AddModifier(new StatModifier(_healthValuePercent, StatModifierType.PercentAdd, this));

        if (_manaValue != 0)
            pInv.Mana.AddModifier(new StatModifier(_manaValue, StatModifierType.Flat, this));
        if (_manaValuePercent != 0)
            pInv.Mana.AddModifier(new StatModifier(_manaValuePercent, StatModifierType.PercentAdd, this));

        if (_armorValue != 0)
            pInv.Armor.AddModifier(new StatModifier(_armorValue, StatModifierType.Flat, this));
        if (_armorValuePercent != 0)
            pInv.Armor.AddModifier(new StatModifier(_armorValuePercent, StatModifierType.PercentAdd, this));

        if (_strenghtValue != 0)
            pInv.Strength.AddModifier(new StatModifier(_strenghtValue, StatModifierType.Flat, this));
        if (_strenghtValuePercent != 0)
            pInv.Strength.AddModifier(new StatModifier(_strenghtValuePercent, StatModifierType.PercentAdd, this));

        if (_intelligenceValue != 0)
            pInv.Intelligence.AddModifier(new StatModifier(_intelligenceValue, StatModifierType.Flat, this));
        if (_intelligenceValuePercent != 0)
            pInv.Intelligence.AddModifier(new StatModifier(_intelligenceValuePercent, StatModifierType.PercentAdd, this));
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
    public float Health { get { return _healthValue; } }
    public float Mana { get { return _manaValue; } }
    public float Armor { get { return _armorValue; } }
    public float Strength { get { return _strenghtValue; } }
    public float Intelligence { get { return _intelligenceValue; } }

    public float HealthPercent { get { return _healthValuePercent; } }
    public float Manapercent { get { return _manaValuePercent; } }
    public float ArmorPercent { get { return _armorValuePercent; } }
    public float StrengthPercent { get { return _strenghtValuePercent; } }
    public float IntelligencePercent { get { return _intelligenceValuePercent; } }
}