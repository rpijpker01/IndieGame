﻿using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [Header("Status Bars")]
    [SerializeField]
    private GameObject _healthBar;
    [SerializeField]
    private GameObject _healthText;
    [SerializeField]
    private GameObject _manaBar;
    [SerializeField]
    private GameObject _manaText;
    [SerializeField]
    [Range(1, 10)]
    private float _barUpdateSpeed = 5;
    [Space]
    [Header("Consumables")]
    [SerializeField] private GameObject _healthPotionParent;
    [SerializeField] private GameObject _manaPotionParent;

    //Consumables
    private Text _hpStacks;
    private Image _hpCooldown;
    private Text _manaStacks;
    private Image _manaCooldown;
    private Inventory _playerInventory;
    [SerializeField]
    private GameObject _dialogBoxText;

    private RectTransform _healthBarTransform;
    private Text _healthValue;
    private RectTransform _manaBarTransform;
    private Text _manaValue;

    private float _healthBarTargetScale;
    private float _manaBarTargetScale;

    private static UICanvas uiCanvas;

    // Use this for initialization
    private void Awake()
    {
        GameController.uiCanvas = this;
        uiCanvas = this;

        _healthBarTransform = _healthBar.GetComponent<RectTransform>();
        _healthValue = _healthText.GetComponent<Text>();
        _manaBarTransform = _manaBar.GetComponent<RectTransform>();
        _manaValue = _manaText.GetComponent<Text>();

        _hpStacks = _healthPotionParent.GetComponentInChildren<Text>();
        Image[] hpI = _healthPotionParent.GetComponentsInChildren<Image>();
        _hpCooldown = hpI[1];
        _hpCooldown.enabled = false;

        _manaStacks = _manaPotionParent.GetComponentInChildren<Text>();
        Image[] manaI = _manaPotionParent.GetComponentsInChildren<Image>();
        _manaCooldown = manaI[1];
        _manaCooldown.enabled = false;

        _playerInventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        uiCanvas = this;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateBars();
        UpdateCooldowns();
        UpdateConsumablesStackValues();
    }

    private void UpdateCooldowns()
    {
        if (Consumable.healthPotionCooldown > 0)
        {
            _hpCooldown.enabled = true;
            _hpCooldown.fillAmount = Consumable.healthPotionCooldown / Consumable.potionCooldown;
        }
        else if (_hpCooldown.enabled)
        {
            _hpCooldown.enabled = false;
        }

        if (Consumable.manaPotionCooldown > 0)
        {
            _manaCooldown.enabled = true;
            _manaCooldown.fillAmount = Consumable.manaPotionCooldown / Consumable.potionCooldown;
        }
        else if (_manaCooldown.enabled)
        {
            _manaCooldown.enabled = false;
        }
    }

    private void UpdateConsumablesStackValues()
    {
        int hpAmount = 0;
        int manaAmount = 0;

        for (int i = 0; i < _playerInventory.SlotsInInventory.Length; i++)
        {
            Consumable c = _playerInventory.SlotsInInventory[i].Item as Consumable;
            if (c == null) continue;

            if (c.ItemType == ConsumableType.HealthPotion)
                hpAmount += _playerInventory.SlotsInInventory[i].Amount;
            else if (c.ItemType == ConsumableType.ManaPotion)
                manaAmount += _playerInventory.SlotsInInventory[i].Amount;
        }

        _hpStacks.text = hpAmount.ToString();
        _manaStacks.text = manaAmount.ToString();
    }

    private void UpdateBars()
    {
        float hp = GameController.playerController.GetHealth(true);
        float maxHp = GameController.maxHealth;
        float mana = GameController.playerController.GetMana(true);
        float maxMana = GameController.maxMana;

        //Get the target scale for the bars
        _healthBarTargetScale = hp / maxHp;
        if (maxMana != 0)
            _manaBarTargetScale = mana / maxMana;
        else
            _manaBarTargetScale = 1;

        //Change bars' scale
        _healthBarTransform.localScale = new Vector3(Mathf.Clamp(Mathf.Lerp(_healthBarTransform.localScale.x, _healthBarTargetScale, Time.deltaTime * _barUpdateSpeed), 0, 1), 1, 1);
        _manaBarTransform.localScale = new Vector3(Mathf.Clamp(Mathf.Lerp(_manaBarTransform.localScale.x, _manaBarTargetScale, Time.deltaTime * _barUpdateSpeed), 0, 1), 1, 1);

        _healthValue.text = string.Format("{0} / {1} ( {2}% )", hp, maxHp, System.Math.Round(_healthBarTargetScale * 100, 1));
        _manaValue.text = string.Format("{0} / {1} ( {2}% )", mana, maxMana, System.Math.Round(_manaBarTargetScale * 100, 1));
    }

    public void CloseDialogBox()
    {
        uiCanvas._dialogBoxText.transform.parent.gameObject.SetActive(false);
    }

    public void OpenDialogBox(string text)
    {
        uiCanvas._dialogBoxText.transform.parent.gameObject.SetActive(true);
        uiCanvas._dialogBoxText.GetComponent<Text>().text = text;
    }
}
