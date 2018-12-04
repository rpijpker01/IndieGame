using UnityEngine.UI;

public class EquipmentSlot : ItemSlot
{
    private EquipmentType _slotType;

    protected override void Awake()
    {
        _image = GetComponent<Image>();
        _amountText = GetComponentInChildren<Text>();
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
            case "gloves1":
                _slotType = EquipmentType.Gloves1;
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

    public override Item Item
    {
        get { return _item; }

        set
        {
            _item = value;

            if (_item == null)
            {
                _image.color = _noAlphaCol;
            }
            else
            {
                _image.sprite = _item.IconWhenEquipped;
                _image.color = _normalCol;
            }

            if (_item is Equippable)
            {
                Equippable e = (Equippable)_item;

                switch (e.name.ToLower().ToCharArray()[0])
                {
                    case 'h':
                        e.ItemType = EquipmentType.Helm;
                        break;
                    case 'c':
                        e.ItemType = EquipmentType.Chest;
                        break;
                    case 'g':
                        e.ItemType = EquipmentType.Gloves;
                        break;
                    case 'p':
                        e.ItemType = EquipmentType.Pants;
                        break;
                    case 'b':
                        e.ItemType = EquipmentType.Boots;
                        break;
                    case 'w':
                        e.ItemType = EquipmentType.Weapon;
                        break;
                    case 'o':
                        e.ItemType = EquipmentType.OffHand;
                        break;
                }
            }
        }
    }

    public override bool CanReceiveItem(Item pItem)
    {
        if (pItem == null)
            return true;

        Equippable eq = pItem as Equippable;
        if (eq != null && eq.ItemType == EquipmentType.Gloves && _slotType == EquipmentType.Gloves1)
            return true;
        else
            return eq != null && eq.ItemType == _slotType;
    }

    public EquipmentType EquipmentType { get { return _slotType; } }
}