using UnityEngine;
using UnityEngine.UI;

public class HighlightGameobject : MonoBehaviour
{
    [SerializeField] private string _textAboveObject;
    [SerializeField] private bool _showTextBox = false;
    [Header("Font Size is 24 by defult")]
    [SerializeField] private int _fontSize;
    private GameObject _textBox;

    private Material _mat;
    private Color _originalColor;

    private void OnEnable()
    {
        _mat = GetComponent<Renderer>().material;

        if (_mat == null)
        {
            Debug.Log(string.Format("GameObject: {0} is missing a material", this.name));
        }
        else
        {
            if (_mat.shader.name == "Toon/Lit Outline")
            {
                GameController.OnMouseOverGameObjectEvent += Highlight;
                GameController.OnMouseAwayFromGameObject += Shade;
                _originalColor = _mat.GetColor("_OutlineColor");

                if (_showTextBox)
                {
                    if (_fontSize == 0)
                        _fontSize = 24;

                    _textBox = Instantiate(Resources.Load<GameObject>("TextAboveHighlightedObjectCanvas"), this.transform);
                    _textBox.GetComponentInChildren<Text>().text = _textAboveObject;
                    _textBox.GetComponentInChildren<Text>().fontSize = _fontSize;
                    _textBox.SetActive(false);

                    GameController.OnMouseOverGameObjectEvent += ShowText;
                    GameController.OnMouseAwayFromGameObject += HideText;
                }

            }
        }
    }

    private void OnDestroy()
    {
        GameController.OnMouseOverGameObjectEvent -= Highlight;
        GameController.OnMouseAwayFromGameObject -= Shade;

        if (_showTextBox)
        {
            GameController.OnMouseOverGameObjectEvent -= ShowText;
            GameController.OnMouseAwayFromGameObject -= HideText;
        }
    }

    private void Highlight(GameObject go)
    {
        if (this.gameObject != go) return;

        switch (this.transform.tag.ToLower())
        {
            case "enemy":
                _mat.SetColor("_OutlineColor", Color.red);
                break;

            default:
                _mat.SetColor("_OutlineColor", Color.cyan);
                break;
        }
    }

    private void Shade(GameObject go)
    {
        if (this.gameObject == go) return;

        _mat.SetColor("_OutlineColor", _originalColor);
    }

    private void ShowText(GameObject go)
    {
        if (this.gameObject != go) return;

        _textBox.SetActive(true);
    }

    private void HideText(GameObject go)
    {
        if (this.gameObject == go) return;

        _textBox.SetActive(false);
    }
}
