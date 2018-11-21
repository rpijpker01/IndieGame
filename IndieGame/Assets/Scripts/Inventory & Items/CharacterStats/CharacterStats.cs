using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace F.CharacterStats
{
    [Serializable]
    public class CharacterStats
    {
        protected float _baseValue;
        protected float _lastBaseValue = float.MinValue;
        protected float _finalValue;

        protected readonly List<StatModifier> _statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        protected bool _isDirty = true;

        public CharacterStats()
        {
            _statModifiers = new List<StatModifier>();
            StatModifiers = _statModifiers.AsReadOnly();
        }

        public CharacterStats(float pBaseValue) : this()
        {
            _baseValue = pBaseValue;
        }

        public virtual void AddModifier(StatModifier pModifier)
        {
            //Add modifiers to the list
            _isDirty = true;
            _statModifiers.Add(pModifier);
            _statModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(StatModifier pModA, StatModifier pModB)
        {
            if (pModA.Order < pModB.Order)
                //ModA comes before ModB
                return -1;
            else if (pModA.Order > pModB.Order)
                //ModA is after ModB
                return 1;
            //Both objects have the same "prority"
            return 0;
        }

        public virtual bool RemoveMidifier(StatModifier pModifier)
        {
            //Try removing modifiers from the list
            if (_statModifiers.Remove(pModifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object pSource)
        {
            bool didRemove = false;

            //Loop through the list in reverse, for better optimization (and less memory mess)
            for (int i = _statModifiers.Count - 1; i >= 0; i--)
            {
                if (_statModifiers[i].Source == pSource)
                {
                    _isDirty = true;
                    didRemove = true;
                    _statModifiers.RemoveAt(i);
                }
            }
            return didRemove;
        }

        public virtual float CalculateFinalValue()
        {
            //Add up all different modifiers to base value
            float finalValue = _baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                StatModifier m = _statModifiers[i];

                if (m.StatType == StatModifierType.Flat)
                    finalValue += m.Value;

                if (m.StatType == StatModifierType.PercentAdd)
                {
                    sumPercentAdd += m.Value;

                    if (i + 1 >= _statModifiers.Count || _statModifiers[i + 1].StatType != StatModifierType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd / 100;
                        sumPercentAdd = 0;
                    }
                }

                if (m.StatType == StatModifierType.PercentMult)
                    finalValue *= 1 + m.Value / 100;
            }

            //Round it to avoid calculation errors
            return (float)Math.Round(finalValue, 0);
        }

        public virtual float Value
        {
            get
            {
                if (_isDirty || _baseValue != _lastBaseValue)
                {
                    _lastBaseValue = _baseValue;
                    _finalValue = CalculateFinalValue();
                    _isDirty = false;
                }
                return _finalValue;
            }
        }
        public float BaseValue { get { return _baseValue; } set { _baseValue = value; } }
    }
}