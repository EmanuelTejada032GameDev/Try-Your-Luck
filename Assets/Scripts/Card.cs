using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image image;
    private int _currencyValue;
    private int _pointsValue;
    private AudioClip _combinationSFX;

    private int _rarity;

    public int Rarity => _rarity;
    public int CurrencyValue => _currencyValue;
    public int PointsValue => _pointsValue;
    public AudioClip CombinationSFX => _combinationSFX;

    public void SetData(CardSO cardData)
    {
        image.sprite = cardData.Sprite;
        _rarity = cardData.Rarity;
        _currencyValue = cardData.CurrencyValue;
        _pointsValue = cardData.PointsValue;
        _combinationSFX = cardData.CombinationSFX;
    }
}
