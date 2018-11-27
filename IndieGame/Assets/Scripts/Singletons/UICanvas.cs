using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject _healthBar;
    [SerializeField]
    private GameObject _manaBar;
    [SerializeField]
    [Range(1, 10)]
    private float _barUpdateSpeed = 5;

    private RectTransform _healthBarTransform;
    private RectTransform _manaBarTransform;

    private float _healthBarTargetScale;
    private float _manaBarTargetScale;

    private float _maxHealth = 100;
    private float _maxMana = 100;

    // Use this for initialization
    private void Awake()
    {
        GameController.uiCanvas = this;

        _healthBarTransform = _healthBar.GetComponent<RectTransform>();
        _manaBarTransform = _manaBar.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        //Get the target scale for the bars
        _healthBarTargetScale = GameController.playerController.GetHealth() / _maxHealth;
        _manaBarTargetScale = GameController.playerController.GetMana() / _maxMana;

        //Change bars' scale
        _healthBarTransform.localScale = new Vector3(Mathf.Clamp(Mathf.Lerp(_healthBarTransform.localScale.x, _healthBarTargetScale, Time.deltaTime * _barUpdateSpeed), 0, 1), 1, 1);
        _manaBarTransform.localScale = new Vector3(Mathf.Clamp(Mathf.Lerp(_manaBarTransform.localScale.x, _manaBarTargetScale, Time.deltaTime * _barUpdateSpeed), 0, 1), 1, 1);
    }
}
