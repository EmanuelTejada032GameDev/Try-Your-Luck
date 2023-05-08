using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
     
    [SerializeField] private GameObject[] _cardSlots = new GameObject[6];
    [SerializeField] private GameObject[] _cardPileGridSlots = new GameObject[3];
    [SerializeField] private List<CardSO> _cardsTypes = new List<CardSO>();
    [SerializeField] private GameObject _cardPrefab;

    public int AvailableCardPileSlots => _cardPileGridSlots.Where(x => x.GetComponent<CardSlot>().IsOccupied == false).Count();


    [SerializeField] private LootPackSO _lootPack;

    // Player UI
    [SerializeField] private int _playerCurrency = 15;
    [SerializeField] private int _playerPoints = 0;
    [SerializeField] private TextMeshProUGUI _playerCurrencyText;
    [SerializeField] private TextMeshProUGUI _playerPointsText;

    private int combinedCardsChain = 0;

    public int PlayerCurrency { get => _playerCurrency; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public List<CardSO> CardsTypes => _cardsTypes;

    private void Start()
    {
        GameStartupConfig();
        Draggable.OnCardCombined += UpdatePlayerStats;
    }



    public void UpdatePlayerStats(object sender , CombinationData combinationData)
    {
        AddPlayerCurrency(combinationData.currencyValue);
        AddPlayerPoints(combinationData.scoreValue);
        combinedCardsChain++;
        if (combinedCardsChain >= 3) {
            AssignNewRandomCardToRandomSlot();
            combinedCardsChain = 0;
        };
    }

    private void AssignNewRandomCardToRandomSlot()
    {
        CardSlot slot = GetRandomFreeSlot();
        if (slot == default) return;

        CardSO cardData = GetRandomCardSOFromLootPack(_lootPack);
        GameObject card = GenerateCardFromData(cardData);

        slot.Occupy(true);
        card.transform.SetParent(slot.transform);
    }


    private CardSlot GetRandomFreeSlot()
    {
        return _cardSlots.Select(x => x.GetComponent<CardSlot>()).FirstOrDefault(cs => cs.IsOccupied == false);
    }


    private CardSO GetRandomCardSOFromLootPack(LootPackSO lootPack)
    {
        return lootPack.GetRandomItem().item;
    }

    private GameObject GenerateCardFromData(CardSO cardData)
    {
        GameObject card = Instantiate(_cardPrefab, transform.position, Quaternion.identity);
        card.GetComponent<Card>().SetData(cardData);
        return card;
    }

    public void AddPlayerCurrency(int amount)
    {
        _playerCurrency += amount;
        _playerCurrencyText.text = _playerCurrency.ToString();
    }

    public void SubstractPlayerCurrency(int amount)
    {
        _playerCurrency -= amount;
        _playerCurrencyText.text = _playerCurrency.ToString();
    }

    public void AddPlayerPoints(int amount)
    {
        _playerPoints += amount;
        _playerPointsText.text = _playerPoints.ToString();

    }

    public void SubstractPlayerPoints(int amount)
    {
        _playerPoints -= amount;
        _playerPointsText.text = _playerPoints.ToString();
    }

    private void GameStartupConfig()
    {
        SetMainGridInitialCards();
        _playerCurrencyText.text = _playerCurrency.ToString(); 
    }

    private void SetMainGridInitialCards()
    {
        int random = Random.Range(4, 6);
        for (int i = 0; i < random; i++)
        {
            AssignNewRandomCardToRandomSlot();
        }

    }

    public void AssignCardToCardPileFreeSlot(GameObject Card)
    {
        foreach(GameObject slot in _cardPileGridSlots)
        {
            var slotComponent = slot.GetComponent<CardSlot>();
            if(!slotComponent.IsOccupied)
            {
                Card.GetComponent<Transform>().SetParent(slot.GetComponent<Transform>());
                slotComponent.Occupy(true);
                break;
            }
        }
    }

    public bool IsCardPileGridFull()
    {
        return _cardPileGridSlots.Select(x => x.GetComponent<CardSlot>().IsOccupied).All(slot => slot == true);
    }

}
