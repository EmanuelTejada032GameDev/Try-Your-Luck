using UnityEngine;

[CreateAssetMenu(fileName ="Card",menuName ="Data/Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _itemName;
    [SerializeField] private int _rarity;

    public Sprite Sprite
    {
        get { return _sprite; }
    }

    public int Rarity
    {
        get { return _rarity; }
    }
}
