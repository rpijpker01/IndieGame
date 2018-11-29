using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10)]
    private float _textSizeDecreaseMultiplier = 5f;
    private float _damageTextSizeDecreaseMultiplier = 1;

    private Vector3 _worldStartingPosition;
    private float _yOffset;

    private int _startingFontSize;

    private Text _textScript;
    private Color _textColor;

    private void Awake()
    {
        _textScript = GetComponent<Text>();
        _startingFontSize = _textScript.fontSize;
    }

    private void Update()
    {
        //Set position of text
        this.transform.position = GameController.camera.WorldToScreenPoint(_worldStartingPosition) + new Vector3(0, _yOffset, 0);

        //Move text up
        _yOffset += 1f;

        //Decrease size of text
        if (_textScript.fontSize > _startingFontSize)
        {
            _textScript.fontSize = (int)Mathf.Lerp(_textScript.fontSize, _startingFontSize, Time.deltaTime * _textSizeDecreaseMultiplier * _damageTextSizeDecreaseMultiplier);
        }

        //Fade text out
        _textColor = _textScript.color;
        _textColor.a -= 0.01f;
        _textScript.color = _textColor;

        if (_textColor.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetStartingPosition(Vector3 worldStartingPosition, float damageValue)
    {
        _worldStartingPosition = worldStartingPosition;
        _textScript.fontSize = 170;

        if (damageValue < 30)
        {
            _damageTextSizeDecreaseMultiplier = 1.5f;
        }
        else
        {
            _damageTextSizeDecreaseMultiplier = 1f - ((damageValue - 30) / 70);
            _damageTextSizeDecreaseMultiplier = Mathf.Clamp(_damageTextSizeDecreaseMultiplier, 0.1f, float.MaxValue);
        }
    }
}
