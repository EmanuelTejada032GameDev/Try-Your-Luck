using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPile : MonoBehaviour
{
    public static CardPile Instance;

    [SerializeField] private LootPackSO commonLootPack;
    [SerializeField] private LootPackSO plusLootPack;
    [SerializeField] private GameObject _commonPackBtn;
    [SerializeField] private GameObject _plusPackBtn;
    [SerializeField] private int _commonPackPrice = 15;
    [SerializeField] private int _plusPackPrice = 30;

    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private AudioClip _cardPileSpawnSFX;
    [SerializeField] private AudioClip _cardPileBtnsSFX;
    [SerializeField] private AudioClip _noCurrencySFX;


    public int CommonPackPrice => _commonPackPrice;
    public int PlusPackPrice => _plusPackPrice;

       

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void GetCommonPackCardsFromPile()
    {
        if (!UIManager.Instance.IsCardPileGridFull())
        {
            if ( UIManager.Instance.PlayerCurrency >= _commonPackPrice)
            {
                UIManager.Instance.PlayOneShotClip(_cardPileBtnsSFX);
                var randomItemsList = commonLootPack.GetRandomItems(UIManager.Instance.AvailableCardPileSlots);
                var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
                SpawnCards(itemsData);
                UIManager.Instance.SubstractPlayerCurrency(_commonPackPrice);
                UIManager.Instance.AddCommonPack();
                UIManager.Instance.AddToSpentCurrency(_commonPackPrice);
                UIManager.Instance.CheckGameOver();
            }
            else
            {
                UIManager.Instance.PlayOneShotClip(_noCurrencySFX);
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
                UIManager.Instance.PlayOneShotClip(_cardPileBtnsSFX);
                var randomItemsList = plusLootPack.GetRandomItems(UIManager.Instance.AvailableCardPileSlots);
                var itemsData = randomItemsList.Select(lootPackItem => lootPackItem.item).ToList();
                SpawnCards(itemsData);
                UIManager.Instance.SubstractPlayerCurrency(_plusPackPrice);
                UIManager.Instance.AddPlusPack();
                UIManager.Instance.AddToSpentCurrency(_plusPackPrice);
                UIManager.Instance.CheckGameOver();
            }
            else
            {
                UIManager.Instance.PlayOneShotClip(_noCurrencySFX);
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
            InstantiatedCardPrefab.transform.localScale = Vector3.zero;
            //yield return new WaitForSeconds(0.20f);
            UIManager.Instance.AssignCardToCardPileFreeSlot(InstantiatedCardPrefab);
        }

    }
}
