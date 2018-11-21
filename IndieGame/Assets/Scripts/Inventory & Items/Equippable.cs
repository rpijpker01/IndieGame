using UnityEngine;

public enum EquipmentType
{
    Helm,
    Chest,
    Gloves,
    Pants,
    Boots,
    Weapon,
    OffHand
}

[CreateAssetMenu]
public class Equippable : Item
{
    [SerializeField] private EquipmentType _equipmentType;

    public EquipmentType ItemType { get { return _equipmentType; } }
}