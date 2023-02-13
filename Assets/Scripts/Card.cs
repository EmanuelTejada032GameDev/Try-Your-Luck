using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private  CardSO cardData;
    [SerializeField] private Image image;
    [SerializeField] private int category;


    private void Awake()
    {
        image.sprite = cardData.Sprite;
        category = cardData.Category;
    }

    //private void Start()
    //{
    //    image.sprite = cardData.Sprite;
    //    category = cardData.Category;
    //}

}
