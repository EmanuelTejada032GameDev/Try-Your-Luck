using UnityEngine;

[CreateAssetMenu(fileName ="Card",menuName ="Data/Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _itemName;
    [SerializeField] private int _rarity;
    [SerializeField] private int _currencyValue;
    [SerializeField] private int _pointsValue;

    public Sprite Sprite
    {
        get { return _sprite; }
    }

    public int Rarity
    {
        get { return _rarity; }
    }

    public int CurrencyValue
    {
        get { return _currencyValue; }
    }

    public int PointsValue
    {
        get { return _pointsValue; }
    }
}
