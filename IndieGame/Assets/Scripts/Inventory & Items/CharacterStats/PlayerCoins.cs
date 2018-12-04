using UnityEngine;
using UnityEngine.UI;

public class PlayerCoins : MonoBehaviour
{
    [SerializeField] private Text _coinsName;
    [SerializeField] private Text _coinsValue;
    [SerializeField] private int _coins;

    private void Awake()
    {
        _coinsName.text = "Coins";
        UpdateCoinValue();
    }

    public void SetCoins(int pValue)
    {
        _coins = pValue;
        UpdateCoinValue();
    }

    public void AddCoins(int pAmount)
    {
        _coins += pAmount;
        UpdateCoinValue();
    }

    public bool TakeCoins(int pAmount)
    {
        if (pAmount > _coins)
        {
            GameController.errorMessage.AddMessage("Not enough coins");
            return false;
        }

        _coins -= pAmount;
        UpdateCoinValue();

        return true;
    }

    private void UpdateCoinValue()
    {
        _coinsValue.text = _coins.ToString();
    }

    public int Coins { get { return _coins; } set { _coins = value; } }
}
