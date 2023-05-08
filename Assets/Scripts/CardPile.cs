using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPile : MonoBehaviour
{
    [SerializeField] private LootPackSO commonLootPack;
    [SerializeField] private LootPackSO plusLootPack;
    [SerializeField] private int _commonPackPrice = 15;
    [SerializeField] private int _plusPackPrice = 30;
    [SerializeField] private GameObject _cardPrefab;


    public void GetCommonPackCardsFromPile()
    {
        if (!UIManager.Instance.IsCardPileGridFull())
        {
            if ( UIManager.Instance.PlayerCurrency >= _commonPackPrice)
            {
                var randomItemsList = commonLootPack.GetRandomItems(UIManager.Instance.AvailableCardPileSlots);
                var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
                SpawnCards(itemsData);
                UIManager.Instance.SubstractPlayerCurrency(_commonPackPrice);
            }
            else
            {
                Debug.Log("Not enough currency");
            }
        }

    }


    public void GetPlusPackCardsFromPile()
    {
        if (!UIManager.Instance.IsCardPileGridFull())
        {
            if (UIManager.Instance.PlayerCurrency >= _plusPackPrice)
            {
                var randomItemsList = plusLootPack.GetRandomItems(UIManager.Instance.AvailableCardPileSlots);
                var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
                SpawnCards(itemsData);
                UIManager.Instance.SubstractPlayerCurrency(_plusPackPrice);
            }
            else
            {
                Debug.Log("Not enough currency");
            }
        }
    }

    public void SpawnCards(List<CardSO> cardsData)
    {
        foreach(CardSO cardData in cardsData) 
        {
            GameObject InstantiatedCardPrefab = Instantiate(_cardPrefab, transform.position, Quaternion.identity);
            InstantiatedCardPrefab.GetComponent<Card>().SetData(cardData);

            UIManager.Instance.AssignCardToCardPileFreeSlot(InstantiatedCardPrefab);
        }

    }
}
