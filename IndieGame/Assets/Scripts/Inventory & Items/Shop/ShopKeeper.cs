using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopKeeper : MonoBehaviour, IPointerClickHandler
{
    public static bool playerIsInShop;

    private Transform _dialogueParent;
    private Text _dialogueText;
    private GameObject _inventory;

    private GameObject _characterPanel;
    private ItemTooltip _itemTooltip;
    private StatTooltip _statTooltip;

    private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        _characterPanel = GameObject.Find("CharacterPanel");

        GameObject dialogue = _characterPanel.transform.parent.GetChild(2).transform.GetChild(0).gameObject;
        _dialogueParent = dialogue.GetComponent<Transform>();
        _dialogueText = dialogue.GetComponentInChildren<Text>();
        _inventory = _characterPanel.transform.parent.GetChild(2).transform.GetChild(1).gameObject;

        _itemTooltip = _characterPanel.transform.parent.GetChild(3).GetComponent<ItemTooltip>();
        _statTooltip = _characterPanel.transform.parent.GetChild(4).GetComponent<StatTooltip>();

        _inventory.SetActive(false);
    }

    public void Update()
    {
        ShopKeeperHandler();
    }

    private void ShopKeeperHandler()
    {
        Vector3 distToPlayer = _playerTransform.position - this.transform.position;

        if (distToPlayer.magnitude <= 5)
        {
            _dialogueParent.position = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 100;

            if (!_dialogueParent.gameObject.activeSelf)
            {
                _dialogueText.text = "'ello there old chap...";
                _dialogueParent.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit = new RaycastHit();
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit);
                if (raycastHit.transform.gameObject == this.gameObject)
                {
                    SetAllWindowsActive();
                }
            }
        }
        else
        {
            if (_dialogueParent.gameObject.activeSelf)
            {
                _dialogueParent.gameObject.SetActive(false);
            }

            if (_inventory.gameObject.activeSelf)
            {
                SetAllWindowsActive(false);
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            SetAllWindowsActive();
        }
    }

    private void SetAllWindowsActive(bool pSetActive = true)
    {
        playerIsInShop = pSetActive;

        _inventory.SetActive(pSetActive);
        _characterPanel.SetActive(pSetActive);

        if (!pSetActive)
        {
            _statTooltip.HideTooltip();
            _itemTooltip.HideTooltip();
        }
    }
}
