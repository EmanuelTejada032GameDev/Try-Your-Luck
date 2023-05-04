using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
     
    [SerializeField] private GameObject[] _cardSlots = new GameObject[6];
    [SerializeField] private GameObject[] _cardPileGridSlots = new GameObject[3];
    [SerializeField] private List<CardSO> _cardsTypes = new List<CardSO>();
    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private int _playerCurrency = 15;
    [SerializeField] private int _playerPoints = 0;
    [SerializeField] private TextMeshProUGUI _playerCurrencyText;
    [SerializeField] private TextMeshProUGUI _playerPointsText;

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
        int counter = 0;
        int random = Random.Range(4, 6);
        do
        {
            GameObject randomSlot = _cardSlots[Random.Range(0, _cardSlots.Length)];
            if (!randomSlot.GetComponent<CardSlot>().IsOccupied)
            {
                AssignNewCardToSlot(randomSlot);
                counter++;
            }
        } while (counter < random);
    }


    private void AssignNewCardToSlot(GameObject randomSlot)
    {
        GameObject InstantiatedCardPrefab = Instantiate(_cardPrefab, randomSlot.GetComponent<Transform>().position, Quaternion.identity);
        InstantiatedCardPrefab.GetComponent<Card>().SetData(_cardsTypes[Random.Range(0, _cardsTypes.Count)]);
        InstantiatedCardPrefab.transform.SetParent(randomSlot.transform, false);

        randomSlot.GetComponent<CardSlot>().Occupy(true);
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

}
