using UnityEngine;

[CreateAssetMenu(fileName ="Card",menuName ="Data/Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private int category;

    public Sprite Sprite
    {
        get { return sprite; }
    }

    public int Category
    {
        get { return category; }
    }
}
