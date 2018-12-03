using UnityEngine;

public enum ConsumableType
{
    HealthPotion,
    ManaPotion
}

[CreateAssetMenu]
public class Consumable : Item
{
    [SerializeField] private int _startCount;
    [SerializeField] private float _effectivenessPercent;

    private ConsumableType _consumableType;

    private float _potionCooldown;
    private float _cooldownTimeLeft;
    private float _healthRecovery;
    private float _manaRecovery;
    private int _maxStacks;

    private void Awake()
    {
        _maxStacks = 20;

        if (_consumableType == ConsumableType.HealthPotion)
            _healthRecovery = _effectivenessPercent;
        if (_consumableType == ConsumableType.ManaPotion)
            _manaRecovery = _effectivenessPercent;
    }

    public void Use(InventoryManager pInv)
    {
        _cooldownTimeLeft = _potionCooldown;
    }

    public void SetEffectiveness()
    {
        _healthRecovery = 0;
        _manaRecovery = 0;

        switch (_consumableType)
        {
            case ConsumableType.HealthPotion:
                _healthRecovery = _effectivenessPercent;
                break;

            case ConsumableType.ManaPotion:
                _manaRecovery = _effectivenessPercent;
                break;
        }
    }

    public float PotionCooldown { get { return _potionCooldown; } set { _potionCooldown = value; } }
    public int MaxStacks { get { return _maxStacks; } }
    public ConsumableType ItemType { get { return _consumableType; } set { _consumableType = value; } }
    public int StartCount { get { return _startCount; } }
    public float HealthRecovery { get { return _healthRecovery; } }
    public float ManaRecovery { get { return _manaRecovery; } }
}