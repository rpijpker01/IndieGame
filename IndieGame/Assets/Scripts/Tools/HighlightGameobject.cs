using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightGameobject : MonoBehaviour
{
    [SerializeField] private float _offset = 0;
    [SerializeField] private string _textAboveObject;
    [SerializeField] private bool _showTextBox = false;
    [Header("Font Size is 24 by defult")]
    [SerializeField] private int _fontSize;
    private GameObject _textBox;

    private Material[] _thisMaterials;
    private Material[] _matsInChildren;
    private List<Material> _matsInChildrenList = new List<Material>();
    private Color _originalColor;

    private List<Material[]> _matslul = new List<Material[]>();

    private bool _eventsAreSubscribed;

    private void OnEnable()
    {
        if (GetComponent<Renderer>() != null)
        {
            Material[] m = GetComponent<Renderer>().materials;
            _thisMaterials = new Material[m.Length];

            for (int i = 0; i < _thisMaterials.Length; i++)
            {
                _thisMaterials[i] = m[i];
            }

            if (_thisMaterials.Length > 0)
            {
                for (int i = 0; i < _thisMaterials.Length; i++)
                {
                    if (_thisMaterials[i] == null) continue;
                    if (_thisMaterials[i].shader.name == "Toon/Lit Outline")
                    {
                        _originalColor = _thisMaterials[i].GetColor("_OutlineColor");
                    }
                }
            }

            if (!_eventsAreSubscribed)
            {
                GameController.OnMouseOverGameObjectEvent += Highlight;
                GameController.OnMouseAwayFromGameObject += Shade;

                _eventsAreSubscribed = true;
            }
        }

        if (GetComponentsInChildren<Renderer>() != null)
        {
            Renderer[] cR = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < cR.Length; i++)
            {
                _matslul.Add(cR[i].materials);
            }

            for (int i = 0; i < _matslul.Count; i++)
            {
                Material[] gimmeMatsPlz = _matslul.ToArray()[i];
                for (int j = 0; j < gimmeMatsPlz.Length; j++)
                {
                    if (gimmeMatsPlz[j].shader.name == "Toon/Lit Outline")
                        _matsInChildrenList.Add(gimmeMatsPlz[j]);
                }
            }

            _matsInChildren = _matsInChildrenList.ToArray();

            if (_matsInChildren.Length > 0)
            {
                for (int i = 0; i < _matsInChildren.Length; i++)
                {
                    if (_matsInChildren[i].shader.name == "Toon/Lit Outline")
                    {
                        _originalColor = _matsInChildren[i].GetColor("_OutlineColor");
                    }
                }
            }

            if (!_eventsAreSubscribed)
            {
                GameController.OnMouseOverGameObjectEvent += Highlight;
                GameController.OnMouseAwayFromGameObject += Shade;

                _eventsAreSubscribed = true;
            }
        }

        if (_showTextBox)
        {
            if (_fontSize == 0)
                _fontSize = 24;

            _textBox = Instantiate(Resources.Load<GameObject>("TextAboveHighlightedObjectCanvas"), this.transform);
            _textBox.GetComponentInChildren<Text>().text = _textAboveObject;
            _textBox.GetComponentInChildren<Text>().fontSize = _fontSize;
            _textBox.GetComponentInChildren<UIFollowParentInWorldSpace>().offset = -_offset;
            _textBox.SetActive(false);

            GameController.OnMouseOverGameObjectEvent += ShowText;
            GameController.OnMouseAwayFromGameObject += HideText;
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
                if (_thisMaterials != null)
                    for (int i = 0; i < _thisMaterials.Length; i++)
                        if (_thisMaterials[i].shader.name == "Toon/Lit Outline")
                            _thisMaterials[i].SetColor("_OutlineColor", Color.red);

                if (_matsInChildren != null)
                    for (int i = 0; i < _matsInChildren.Length; i++)
                        _matsInChildren[i].SetColor("_OutlineColor", Color.red);
                break;

            default:
                if (_thisMaterials != null)
                    for (int i = 0; i < _thisMaterials.Length; i++)
                        if (_thisMaterials[i].shader.name == "Toon/Lit Outline")
                            _thisMaterials[i].SetColor("_OutlineColor", Color.cyan);

                if (_matsInChildren != null)
                    for (int i = 0; i < _matsInChildren.Length; i++)
                        _matsInChildren[i].SetColor("_OutlineColor", Color.cyan);
                break;
        }
    }

    private void Shade(GameObject go)
    {
        if (this.gameObject == go) return;

        if (_thisMaterials != null)
            for (int i = 0; i < _thisMaterials.Length; i++)
                if (_thisMaterials[i].shader.name == "Toon/Lit Outline")
                    _thisMaterials[i].SetColor("_OutlineColor", _originalColor);

        if (_matsInChildren != null)
            for (int i = 0; i < _matsInChildren.Length; i++)
                _matsInChildren[i].SetColor("_OutlineColor", _originalColor);
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
