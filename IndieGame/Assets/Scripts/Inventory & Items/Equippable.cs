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

    [SerializeField] private float HealthValue;
    [SerializeField] private float ManaValue;
    [SerializeField] private float ArmorValue;
    [SerializeField] private float StrenghtValue;
    [SerializeField] private float IntelligenceValue;
    [Space]
    [SerializeField] private float HealthValuePercent;
    [SerializeField] private float ManaValuePercent;
    [SerializeField] private float ArmorValuePercent;
    [SerializeField] private float StrenghtValuePercent;
    [SerializeField] private float IntelligenceValuePercent;

    public void Equip(InventoryManager pInv)
    {
        if (HealthValue != 0)
            pInv.Health.AddModifier(new StatModifier(HealthValue, StatModifierType.Flat, this));
        if (ManaValue != 0)
            pInv.Mana.AddModifier(new StatModifier(ManaValue, StatModifierType.Flat, this));
        if (ArmorValue != 0)
            pInv.Armor.AddModifier(new StatModifier(ArmorValue, StatModifierType.Flat, this));
        if (StrenghtValue != 0)
            pInv.Strength.AddModifier(new StatModifier(StrenghtValue, StatModifierType.Flat, this));
        if (IntelligenceValue != 0)
            pInv.Intelligence.AddModifier(new StatModifier(IntelligenceValue, StatModifierType.Flat, this));

        if (HealthValuePercent != 0)
            pInv.Health.AddModifier(new StatModifier(HealthValuePercent, StatModifierType.PercentAdd, this));
        if (ManaValuePercent != 0)
            pInv.Mana.AddModifier(new StatModifier(ManaValuePercent, StatModifierType.PercentAdd, this));
        if (ArmorValuePercent != 0)
            pInv.Armor.AddModifier(new StatModifier(ArmorValuePercent, StatModifierType.PercentAdd, this));
        if (StrenghtValuePercent != 0)
            pInv.Strength.AddModifier(new StatModifier(StrenghtValuePercent, StatModifierType.PercentAdd, this));
        if (IntelligenceValuePercent != 0)
            pInv.Intelligence.AddModifier(new StatModifier(IntelligenceValuePercent, StatModifierType.PercentAdd, this));
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
}