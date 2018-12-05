using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
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

        _healthBarTransform = _healthBar.GetComponent<RectTransform>();
        _healthValue = _healthText.GetComponent<Text>();
        _manaBarTransform = _manaBar.GetComponent<RectTransform>();
        _manaValue = _manaText.GetComponent<Text>();

        uiCanvas = this;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        float hp = GameController.playerController.GetHealth(true);
        float maxHp = GameController.maxHealth;
        float mana = GameController.playerController.GetMana(true);
        float maxMana = GameController.maxMana;

        //Get the target scale for the bars
        _healthBarTargetScale = hp / maxHp;
        _manaBarTargetScale = mana / maxMana;

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
