using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private int _currencyValue;
    [SerializeField] private int _pointsValue;
    [SerializeField] private Image logo;
    private int _rarity;

    public int Rarity => _rarity;
    public int CurrencyValue => _currencyValue;
    public int PointsValue => _pointsValue;

    public void SetData(CardSO cardData)
    {
        image.sprite = cardData.Sprite;
        _rarity = cardData.Rarity;
        _currencyValue = cardData.CurrencyValue;
        _pointsValue = cardData.PointsValue;
    }
}
