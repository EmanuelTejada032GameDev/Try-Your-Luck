using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDragTransform;
    [HideInInspector] public CardSlot currentCardGridSlot;

    public static event EventHandler<CombinationData> OnCardCombined;
    

    private Image itemImage;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentCardGridSlot = transform.parent.GameObject().GetComponent<CardSlot>();
        parentAfterDragTransform = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool isCard = CheckIsCard(eventData.pointerEnter);
        bool isSpecialCard = UIManager.Instance.CheckIsSpecialCardRarity(eventData.pointerDrag.GetComponent<Card>().Rarity);

        if (isSpecialCard)
        {
            UIManager.Instance.SpecialCardUse();
            Destroy(gameObject);
        }

        if (isCard)
            TryCardCombination(eventData);

        transform.SetParent(parentAfterDragTransform);
        itemImage.raycastTarget = true;
    }

    private void TryCardCombination(PointerEventData eventData)
    {
            bool isCombinable = CheckIsCombinableCard(eventData.pointerDrag, eventData.pointerEnter);
            if (isCombinable)
            {
                CombineCards(eventData.pointerDrag, eventData.pointerEnter);
            }
    }

    private void CombineCards(GameObject droppedCard, GameObject cardInSlot)
    {
        Card card = droppedCard.GetComponent<Card>();

        bool isMaxRarity = CheckMaxCardRarity(card.Rarity ,UIManager.Instance.CardsTypes);

        if (card.CombinationSFX != default)
            UIManager.Instance.PlayOneShotClip(card.CombinationSFX);

        if (isMaxRarity)
        {
            UIManager.Instance.CheckForSpecialCard();
            OnCardCombined?.Invoke(this, new CombinationData { currencyValue = card.CurrencyValue, scoreValue = card.PointsValue});
            cardInSlot.transform.parent.gameObject.GetComponent<CardSlot>().Occupy(false);
            Destroy(cardInSlot);
        }
        else
        {
            CardSO nextLevelCard = UIManager.Instance.CardsTypes.Where(x => x.Rarity == (card.Rarity + 1)).SingleOrDefault();
            cardInSlot.GetComponent<Card>().SetData(nextLevelCard);
            OnCardCombined?.Invoke(this, new CombinationData { currencyValue = card.CurrencyValue, scoreValue = card.PointsValue });
        }

        Destroy(gameObject);
        currentCardGridSlot.Occupy(false);
    }

    private bool CheckMaxCardRarity(int currentCardRarity , List<CardSO> cardsTypes)
    {
        return cardsTypes.Select(x => x.Rarity).LastOrDefault() == currentCardRarity;
    }

    private bool CheckIsCombinableCard(GameObject droppedCard, GameObject cardInSlot)
    {
        int droppedCardRarity = droppedCard.GetComponent<Card>().Rarity;
        int cardInSlotRarity = cardInSlot.GetComponent<Card>().Rarity;

        return (droppedCardRarity == cardInSlotRarity)? true: false;    
    }

    private bool CheckIsCard(GameObject pointerEnter)
    {
        return (pointerEnter?.gameObject.GetComponent<Draggable>() != default) ? true : false;
    }
}
