using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    //Enum for determining what the canvas should be doing
    private enum FadeState
    {
        FadingIn,
        FadingOut,
        None,
    }
    private FadeState _fadeState = FadeState.None;

    [SerializeField]
    private float _fadeSpeed = 5;

    private GameObject _blackground;
    private Image _blackgroundImage;

    // Use this for initialization
    private void Awake()
    {
        GameController.mainCanvas = this;

        //Get the blackground
        _blackground = transform.GetChild(0).gameObject;
        _blackgroundImage = _blackground.GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_fadeState)
        {
            case FadeState.FadingIn:
                _blackgroundImage.color = new Color(_blackgroundImage.color.r, _blackgroundImage.color.g, _blackgroundImage.color.b, Mathf.Lerp(_blackgroundImage.color.a, 1, Time.deltaTime * _fadeSpeed * 8));
                if (_blackgroundImage.color.a > 0.99f && _blackgroundImage.color.a < 1.01f)
                {
                    _blackgroundImage.color = new Color(_blackgroundImage.color.r, _blackgroundImage.color.g, _blackgroundImage.color.b, 1);
                    _fadeState = FadeState.None;
                }
                break;
            case FadeState.FadingOut:
                if (_blackgroundImage.color.a > -0.01f && _blackgroundImage.color.a < 0.01f)
                {
                    _blackground.SetActive(false);
                    _blackgroundImage.color = new Color(_blackgroundImage.color.r, _blackgroundImage.color.g, _blackgroundImage.color.b, 0);
                    _fadeState = FadeState.None;
                    GameController.playerController.SetHealth(100);
                    GameController.playerController.died = false;
                    GameController.player.GetComponent<Collider>().enabled = true;
                }
                _blackgroundImage.color = new Color(_blackgroundImage.color.r, _blackgroundImage.color.g, _blackgroundImage.color.b, Mathf.Lerp(_blackgroundImage.color.a, 0, Time.deltaTime * _fadeSpeed));
                break;
            default:
                break;
        }
    }

    public void FadeToBlack()
    {
        _blackground.SetActive(true);
        _fadeState = FadeState.FadingIn;
    }

    public void FadeOutBlack()
    {
        _fadeState = FadeState.FadingOut;
    }

    public float GetBlackgroundAlpha()
    {
        return _blackgroundImage.color.a;
    }
}
