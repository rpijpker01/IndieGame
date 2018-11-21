using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    private Text _statName;
    private Text _statValue;

    private void Awake()
    {
        Text[] t = GetComponentsInChildren<Text>();
        _statName = t[0];
        _statValue = t[1];
    }

    public Text Name { get { return _statName; } set { _statName = value; } }
    public Text Value { get { return _statValue; } set { _statValue = value; } }
}