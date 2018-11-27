using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    [SerializeField] private float _maxFadeTime = 5;

    private Text _text;
    private float _fadeTime;

    private void Awake()
    {
        GameController.errorMessage = this;
        _text = GetComponent<Text>();
        _text.text = "";
    }

    private void Update()
    {
        if (_fadeTime <= 0) return;

        _fadeTime -= Time.deltaTime;

        if (_fadeTime > _maxFadeTime * 0.75f) return;

        SetAlpha(_fadeTime / _maxFadeTime);

        if (_fadeTime - Time.deltaTime > 0) return;

        _text.text = "";
    }

    public void DisplayMessage(string pText)
    {
        _text.text = pText;
        SetAlpha(1);
        _fadeTime = _maxFadeTime;
    }

    private void SetAlpha(float pAlpha)
    {
        Color a = _text.color;
        a.a = pAlpha;
        _text.color = a;
    }
}
