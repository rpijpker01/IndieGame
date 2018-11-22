using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using F.CharacterStats;

public class StatTooltip : MonoBehaviour
{
    private Text _name;
    private Text _mods;
    private RectTransform _modsTransform;
    private Text _positiveMod;
    private RectTransform _positiveTransform;
    private Text _negativeMod;
    private RectTransform _negativeTransform;

    private RectTransform _tooltipTransform;

    private StringBuilder _sbStats = new StringBuilder();
    private StringBuilder _sbPositive = new StringBuilder();
    private StringBuilder _sbNegative = new StringBuilder();

    private Vector2 _rSize;

    private void Awake()
    {
        Text[] t = GetComponentsInChildren<Text>();
        _name = t[0];
        _mods = t[2];

        Text[] stats = _mods.GetComponentsInChildren<Text>();
        _positiveMod = stats[1];
        _negativeMod = stats[2];

        RectTransform[] tr = _mods.GetComponentsInChildren<RectTransform>();
        _modsTransform = tr[0];
        _positiveTransform = tr[1];
        _negativeTransform = tr[2];

        _tooltipTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        HideTooltip();
    }

    private void Update()
    {
        if (!this.enabled) return;

        _tooltipTransform.position = Input.mousePosition;

        _rSize = new Vector2(_modsTransform.rect.width * 0.25f, _modsTransform.rect.height);
        _positiveTransform.sizeDelta = _rSize;
        _negativeTransform.sizeDelta = _rSize;
    }

    public void ShowTooltip(CharacterStats pStat, string pName)
    {
        _name.text = string.Format("{0}\t\t{1}", pName, pStat.Value);

        GetStatModifiers(pStat);

        _mods.text = _sbStats.ToString();
        _positiveMod.text = _sbPositive.ToString();
        _negativeMod.text = _sbNegative.ToString();

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void GetStatModifiers(CharacterStats pStats)
    {
        _sbStats.Length = 0;
        _sbPositive.Length = 0;
        _sbNegative.Length = 0;

        if (pStats.StatModifiers.Count == 0)
        {
            _sbStats.Append("none");
            _mods.alignment = TextAnchor.MiddleCenter;
            return;
        }
        _mods.alignment = TextAnchor.MiddleLeft;

        List<EquipmentType> sm = new List<EquipmentType>();

        foreach (StatModifier mod in pStats.StatModifiers)
        {
            if (mod.Value == 0) continue;

            if (_sbPositive.Length > 0)
            {
                _sbPositive.AppendLine();
                _sbNegative.AppendLine();
            }

            Equippable item = mod.Source as Equippable;

            if (item != null)
                sm.Add(item.ItemType);

            if (mod.Value > 0)
            {
                _sbPositive.Append("+");
                _sbPositive.Append(mod.Value);
                if (mod.StatType != StatModifierType.Flat)
                    _sbPositive.Append("%");
            }
            else
            {
                _sbNegative.Append(mod.Value);
                if (mod.StatType != StatModifierType.Flat)
                    _sbNegative.Append("%");
            }
        }

        //Sort the list so items are sorter alphabetically in the tooltip
        sm.Sort();
        foreach (EquipmentType type in sm)
        {
            if (_sbStats.Length > 0)
                _sbStats.AppendLine();

            _sbStats.Append(type);
        }
    }
}
