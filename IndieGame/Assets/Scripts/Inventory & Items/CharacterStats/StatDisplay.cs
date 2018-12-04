﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using F.CharacterStats;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterStats _stats;
    public CharacterStats Stats
    {
        get { return _stats; }
        set
        {
            _stats = value;
            UpdateStatValue();
        }
    }

    private Text _statName;
    private Text _statValue;
    private StatTooltip _tooltip;

    private void Awake()
    {
        Text[] t = GetComponentsInChildren<Text>();
        _statName = t[0];
        _statValue = t[1];

        _tooltip = GameObject.Find("StatTooltip").GetComponent<StatTooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltip.ShowTooltip(Stats, _statName.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.HideTooltip();
    }

    public void UpdateStatValue()
    {
        _statValue.text = _stats.Value.ToString();

        switch (_statName.text.ToLower())
        {
            case "max health":
                GameController.maxHealth = _stats.Value;
                break;
            case "max mana":
                GameController.maxMana = _stats.Value;
                break;
            case "armor":
                GameController.armor = _stats.Value;
                break;
            case "strength":
                GameController.strength = _stats.Value;
                break;
            case "intelligence":
                GameController.intelligence = _stats.Value;
                break;
        }
    }

    public Text Name { get { return _statName; } set { _statName = value; } }
    public Text Value { get { return _statValue; } set { _statValue = value; } }
}