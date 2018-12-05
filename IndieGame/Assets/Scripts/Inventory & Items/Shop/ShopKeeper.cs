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
        _characterPanel = GameObject.Find("CharacterPanel");

        GameObject dialogue = _characterPanel.transform.parent.GetChild(3).transform.GetChild(0).gameObject;
        _dialogueParent = dialogue.GetComponent<Transform>();
        _dialogueText = dialogue.GetComponentInChildren<Text>();
        _inventory = _characterPanel.transform.parent.GetChild(3).transform.GetChild(1).gameObject;

        _itemTooltip = _characterPanel.transform.parent.GetChild(4).GetComponent<ItemTooltip>();
        _statTooltip = _characterPanel.transform.parent.GetChild(5).GetComponent<StatTooltip>();

        GameController.OnMouseLeftClickGameObject += OpenShop;

        _inventory.SetActive(false);
    }

    private void Start()
    {
        _playerTransform = GameController.player.transform;
    }

    public void Update()
    {
        ShopKeeperHandler();
    }

    private void ShopKeeperHandler()
    {
        Vector3 distToPlayer = _playerTransform.position - this.transform.position;

        if (distToPlayer.magnitude < 5)
        {
            _dialogueParent.position = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 100;

            if (!_dialogueParent.gameObject.activeSelf)
            {
                _dialogueText.text = "'ello there old chap...";
                _dialogueParent.gameObject.SetActive(true);
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

    private void OpenShop(GameObject go)
    {
        if (this.gameObject != go) return;

        SetAllWindowsActive();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            SetAllWindowsActive();
            if (OlChapBehaviour.GetQuestProgression() == 3)
            {
                OlChapBehaviour.ContinuePorgression();
            }
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
