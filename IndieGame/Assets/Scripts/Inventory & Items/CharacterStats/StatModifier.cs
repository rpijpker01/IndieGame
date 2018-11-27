namespace F.CharacterStats
{
    public enum StatModifierType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }

    public class StatModifier
    {
        protected readonly StatModifierType _type;
        protected readonly float _value;
        protected readonly int _order;
        protected readonly object _source;

        public StatModifier(float pValue, StatModifierType pType, int pOrder, object pSource)
        {
            _value = pValue;
            _type = pType;
            _order = pOrder;
            _source = pSource;
        }

        public StatModifier(float pValue, StatModifierType pType) : this(pValue, pType, (int)pType, null) { }

        public StatModifier(float pValue, StatModifierType pType, int pOrder) : this(pValue, pType, pOrder, null) { }

        public StatModifier(float pValue, StatModifierType pType, object pSource) : this(pValue, pType, (int)pType, pSource) { }

        public int Order { get { return _order; } }
        public float Value { get { return _value; } }
        public object Source { get { return _source; } }
        public StatModifierType StatType { get { return _type; } }
    }
}