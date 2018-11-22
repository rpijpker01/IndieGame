using UnityEngine;
using F.CharacterStats;

public class StatPanel : MonoBehaviour
{
    private StatDisplay[] _statDisplays;
    private string[] _statNames;

    private CharacterStats[] _stats;

    private void Awake()
    {
        _statDisplays = GetComponentsInChildren<StatDisplay>();
        _statNames = new string[] { "Max Health", "Max Mana", "Armor", "Strength", "Intelligence" };

        UpdateStatNames();
    }

    public void SetStats(params CharacterStats[] pCharStats)
    {
        _stats = pCharStats;

        if (_stats.Length > _statDisplays.Length) return;

        for (int i = 0; i < _statDisplays.Length; i++)
        {
            _statDisplays[i].gameObject.SetActive(i < _stats.Length);

            if (i < _stats.Length)
                _statDisplays[i].Stats = _stats[i];
        }
    }

    public void UpdateStatValues()
    {
        for (int i = 0; i < _stats.Length; i++)
        {
            _statDisplays[i].UpdateStatValue();
        }
    }

    private void UpdateStatNames()
    {
        for (int i = 0; i < _statNames.Length; i++)
        {
            _statDisplays[i].Name.text = _statNames[i];
        }
    }
}
