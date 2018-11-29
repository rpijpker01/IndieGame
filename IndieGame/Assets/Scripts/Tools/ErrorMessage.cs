using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private float _maxFadeTime = 5;

    private void Awake()
    {
        GameController.errorMessage = this;
    }

    public void AddMessage(string pText, Color? pCol = null)
    {
        Text t = Instantiate(_text, this.transform);
        t.transform.SetAsFirstSibling();
        t.color = pCol ?? Color.red;
        t.text = pText;
    }

    public float MaxFadeTime { get { return _maxFadeTime; } }
}
