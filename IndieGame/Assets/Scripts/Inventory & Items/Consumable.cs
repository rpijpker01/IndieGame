using System.Collections;
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
    [SerializeField] private float _duration;

    private ConsumableType _consumableType;

    public static float potionCooldown;
    public static float healthPotionCooldown;
    public static float manaPotionCooldown;
    private float _healthRecovery;
    private float _manaRecovery;
    private int _maxStacks;

    public bool Use()
    {
        potionCooldown = 30;

        if (_consumableType == ConsumableType.HealthPotion)
        {
            if (healthPotionCooldown > 0)
            {
                GameController.errorMessage.AddMessage(string.Format("{0} is on cooldown", _name));
                return false;
            }
            GameController.playerController.Heal(_healthRecovery, _duration);
            healthPotionCooldown = potionCooldown;
            return true;
        }
        else if (_consumableType == ConsumableType.ManaPotion)
        {
            if (manaPotionCooldown > 0)
            {
                GameController.errorMessage.AddMessage(string.Format("{0} is on cooldown", _name));
                return false;
            }
            GameController.playerController.Drink(_manaRecovery, _duration);
            manaPotionCooldown = potionCooldown;
            return true;
        }

        return false;
    }

    public void SetEffectiveness()
    {
        _healthRecovery = 0;
        _manaRecovery = 0;
        _maxStacks = 20;

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

    public int MaxStacks { get { return _maxStacks; } }
    public ConsumableType ItemType { get { return _consumableType; } set { _consumableType = value; } }
    public int StartCount { get { return _startCount; } }
    public float HealthRecovery { get { return _healthRecovery; } }
    public float ManaRecovery { get { return _manaRecovery; } }
}