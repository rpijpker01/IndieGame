using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] protected string _name;
    [SerializeField] protected Sprite _icon;

    public string Name { get { return _name; } set { _name = value; } }
    public Sprite Icon { get { return _icon; } set { _icon = value; } }
}