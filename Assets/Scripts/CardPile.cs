using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPile : MonoBehaviour
{
    [SerializeField] private LootPackSO commonLootPack;
    [SerializeField] private LootPackSO plusLootPack;
    [SerializeField] private int _commonPackPrice = 15;
    [SerializeField] private int _plusPackPrice = 35;
    [SerializeField] private GameObject _cardPrefab;



    public void GetCommonPackCardsFromPile()
    {
        if( UIManager.Instance.PlayerCurrency >= _commonPackPrice)
        {
            var randomItemsList = commonLootPack.GetRandomItems(3);
            var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
            SpawnCards(itemsData);
            UIManager.Instance.SubstractPlayerCurrency(_commonPackPrice);
        }
        else
        {
            Debug.Log("Not enough currency");
        }
    }


    public void GetPlusPackCardsFromPile()
    {
        if (UIManager.Instance.PlayerCurrency > _plusPackPrice)
        {
            var randomItemsList = plusLootPack.GetRandomItems(3);
            var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
            SpawnCards(itemsData);
            UIManager.Instance.SubstractPlayerCurrency(_plusPackPrice);
        }
        else
        {
            Debug.Log("Not enough currency");
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
