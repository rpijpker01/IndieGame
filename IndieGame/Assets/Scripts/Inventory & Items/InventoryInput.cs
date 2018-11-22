using UnityEngine;

public class InventoryInput : MonoBehaviour
{
    private GameObject _charPanel;
    private ItemTooltip _itemTooltip;
    private StatTooltip _statTooltip;

    private void Awake()
    {
        _charPanel = GameObject.Find("CharacterPanel");
        _itemTooltip = GameObject.Find("ItemTooltip").GetComponent<ItemTooltip>();
        _statTooltip = GameObject.Find("StatTooltip").GetComponent<StatTooltip>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _charPanel.SetActive(!_charPanel.activeSelf);
            _itemTooltip.HideTooltip();
            _statTooltip.HideTooltip();
        }
    }
}
