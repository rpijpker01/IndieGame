using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
    private Text _text;
    private float _maxFadeTime;
    private float _fadeTime;

    private void Start()
    {
        _text = GetComponent<Text>();
        _maxFadeTime = transform.parent.GetComponent<ErrorMessage>().MaxFadeTime;
        _fadeTime = _maxFadeTime;
    }

    private void Update()
    {
        if (_fadeTime < 0) return;

        _fadeTime -= Time.deltaTime;

        SetAlpha(_fadeTime / _maxFadeTime);

        if (_fadeTime - Time.deltaTime > 0) return;

        Destroy(this.gameObject);
    }

    private void SetAlpha(float pAlpha)
    {
        Color a = _text.color;
        a.a = pAlpha;
        _text.color = a;
    }
}
