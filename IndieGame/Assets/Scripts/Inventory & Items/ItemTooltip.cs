using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    private Text _name;
    private Text _type;
    private Text _stats;
    private Text _itemValue;
    private RectTransform _statsTransform;
    private Text _positiveStat;
    private RectTransform _positiveTransform;
    private Text _negativeStat;
    private RectTransform _negativeTransform;

    private RectTransform _tooltipTransform;

    private StringBuilder _stringBuilder = new StringBuilder();
    private StringBuilder _positive = new StringBuilder();
    private StringBuilder _negative = new StringBuilder();

    private Vector2 _rSize;

    private void Awake()
    {
        Text[] t = GetComponentsInChildren<Text>();
        _name = t[0];
        _type = t[1];
        _stats = t[2];
        _itemValue = t[5];

        Text[] stats = _stats.GetComponentsInChildren<Text>();
        _positiveStat = stats[1];
        _negativeStat = stats[2];

        RectTransform[] tr = _stats.GetComponentsInChildren<RectTransform>();
        _statsTransform = tr[0];
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

        if (Input.mousePosition.x + _tooltipTransform.rect.width >= Screen.width - 25)
            _tooltipTransform.position -= new Vector3(_tooltipTransform.rect.width, 0);

        if (Input.mousePosition.y - _tooltipTransform.rect.height <= 25)
            _tooltipTransform.position += new Vector3(0, _tooltipTransform.rect.height);

        _rSize = new Vector2(_statsTransform.rect.width * 0.25f, _statsTransform.rect.height);
        _positiveTransform.sizeDelta = _rSize;
        _negativeTransform.sizeDelta = _rSize;
    }

    public void ShowTooltip(Item pItem)
    {
        if (pItem is Equippable)
            ShowTooltip((Equippable)pItem);
        else if (pItem is Consumable)
            ShowTooltip((Consumable)pItem);
    }

    private void ShowTooltip(Consumable pItem)
    {
        _name.text = pItem.Name;
        if (pItem.ItemType == ConsumableType.HealthPotion || pItem.ItemType == ConsumableType.ManaPotion)
            _type.text = "Potion";

        _stringBuilder.Length = 0;
        _negative.Length = 0;
        _positive.Length = 0;

        AddStat(pItem.HealthRecovery, "Heals for\n% of Max Health\nover 10 seconds", true);
        AddStat(pItem.ManaRecovery, "Recovers\n% of Max Mana\nover 10 seconds", true);

        _stats.text = _stringBuilder.ToString();
        _positiveStat.text = _positive.ToString();
        _negativeStat.text = _negative.ToString();

        if (pItem.IsInShop)
            _itemValue.text = string.Format("buy price:\t\t{0}", pItem.Value);
        else
            _itemValue.text = string.Format("sell price:\t\t{0}", pItem.Value);

        _tooltipTransform.position = Input.mousePosition;

        gameObject.SetActive(true);
    }

    private void ShowTooltip(Equippable pItem)
    {
        _name.text = pItem.Name;
        _type.text = pItem.ItemType.ToString();

        if (pItem.IsInShop)
            _itemValue.text = string.Format("buy price:\t\t{0}", pItem.Value);
        else
            _itemValue.text = string.Format("sell price:\t\t{0}", pItem.Value);

        _stringBuilder.Length = 0;
        _positive.Length = 0;
        _negative.Length = 0;

        AddStat(pItem.Health, "Max Health");
        AddStat(pItem.HealthPercent, "% Max Health", true);

        AddStat(pItem.Mana, "Mana");
        AddStat(pItem.Manapercent, "% Max Mana", true);

        AddStat(pItem.Armor, "Armor");
        AddStat(pItem.ArmorPercent, "% Armor", true);

        AddStat(pItem.Strength, "Strength");
        AddStat(pItem.StrengthPercent, "% Strength", true);

        AddStat(pItem.Intelligence, "Intelligence");
        AddStat(pItem.IntelligencePercent, "% Intelligence", true);

        _stats.text = _stringBuilder.ToString();

        _positiveStat.text = _positive.ToString();
        _negativeStat.text = _negative.ToString();

        _tooltipTransform.position = Input.mousePosition;

        gameObject.SetActive(true);
    }

    public void HideTooltip(Item pItem = null)
    {
        gameObject.SetActive(false);
    }

    private void AddStat(float pValue, string pName, bool pIsPercent = false)
    {
        if (pValue == 0) return;

        if (_stringBuilder.Length > 0)
        {
            _stringBuilder.AppendLine();
            _positive.AppendLine();
            _negative.AppendLine();
        }

        _stringBuilder.Append(pName);

        if (pValue > 0)
        {
            if (pIsPercent)
            {
                _positive.Append("\t");
                _positive.Append("+");
                _positive.Append(pValue);
                _positive.Append("%");
            }
            else
            {
                _positive.Append("+");
                _positive.Append(pValue);
            }
        }
        else
        {
            if (pIsPercent)
            {
                _negative.Append("\t");
                _negative.Append(pValue);
                _negative.Append("%");
            }
            else
            {
                _negative.Append(pValue);
            }
        }
    }
}
