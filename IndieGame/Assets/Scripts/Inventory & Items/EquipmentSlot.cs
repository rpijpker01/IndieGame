using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    private EquipmentType _slotType;

    protected override void Awake()
    {
        base.Awake();

        //Assign each equippable slot with the type it can hold
        AssignSlotType();
    }

    private void AssignSlotType()
    {
        //Assign each equippable slot with the type it can hold
        switch (gameObject.name.ToLower())
        {
            case "helm":
            _slotType = EquipmentType.Helm;
            break;
            case "chest":
            _slotType = EquipmentType.Chest;
            break;
            case "gloves":
            _slotType = EquipmentType.Gloves;
            break;
            case "pants":
            _slotType = EquipmentType.Pants;
            break;
            case "boots":
            _slotType = EquipmentType.Boots;
            break;
            case "weapon":
            _slotType = EquipmentType.Weapon;
            break;
            case "off-hand":
            _slotType = EquipmentType.OffHand;
            break;
        }
    }

    public EquipmentType EquipmentType { get { return _slotType; } }
}