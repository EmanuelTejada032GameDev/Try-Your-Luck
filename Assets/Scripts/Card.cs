using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image image;
    private int _rarity;

    public int Rarity => _rarity;

    public void SetData(CardSO cardData)
    {
        image.sprite = cardData.Sprite;
        _rarity = cardData.Rarity;
    }

    public void UpgradeCard()
    {
        
    }
}
