using System;

public static class ExtensionMethods
{
    public static T Clamp<T>(this T pValue, T pMin, T pMax) where T : IComparable<T>
    {
        if (pValue.CompareTo(pMin) < 0) return pMin;
        else if (pValue.CompareTo(pMax) > 0) return pMax;
        else return pValue;
    }
}
