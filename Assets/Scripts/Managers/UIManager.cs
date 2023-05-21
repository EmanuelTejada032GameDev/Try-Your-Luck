using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
     
    [SerializeField] private GameObject[] _cardSlots = new GameObject[6];
    [SerializeField] private GameObject[] _cardPileGridSlots = new GameObject[3];
    [SerializeField] private List<CardSO> _cardsTypes = new List<CardSO>();
    [SerializeField] private GameObject _cardPrefab;

    //Special Card
    [SerializeField] private CardSlot _specialCardSlot;
    [SerializeField] private CardSO _specialCardData;
    [SerializeField] private AudioClip _specialCardSpawnedSFX;


    [SerializeField] private LootPackSO _lootPack;

    // Player UI
    [SerializeField] private int _playerCurrency = 15;
    [SerializeField] private int _playerPoints = 0;
    [SerializeField] private TextMeshProUGUI _playerCurrencyText;
    [SerializeField] private TextMeshProUGUI _playerPointsText;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _spawnPopUpSFX;
    [SerializeField] private AudioClip _altSpawnPopUpSFX;
    [SerializeField] private AudioClip _destroyCardSFX;

    public int AvailableCardPileSlots => _cardPileGridSlots.Where(x => x.GetComponent<CardSlot>().IsOccupied == false).Count();
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
        _audioSource = GetComponent<AudioSource>(); 
        Draggable.OnCardCombined += UpdatePlayerStats;
        GameStartupConfig();
    }





    private CardSlot GetRandomFreeSlot()
    {
        return _cardSlots.Select(x => x.GetComponent<CardSlot>()).FirstOrDefault(cs => cs.IsOccupied == false);
    }

    private CardSlot GetRandomOccupiedSlot()
    {
        bool isOccupied = false;
        CardSlot cardSlot = default;
        if(_cardSlots.Select(x => x.GetComponent<CardSlot>()).Any(cs => cs.IsOccupied == true))
        {
            do
            {
                GameObject cardSlotGO = _cardSlots[Random.Range(0, _cardSlots.Length)];
                cardSlot = cardSlotGO.GetComponent<CardSlot>();
                isOccupied = cardSlot.IsOccupied;
            } while (isOccupied == false);
        }
        return cardSlot;
    }

    public void AddPlayerCurrency(int amount)
    {
        _playerCurrency += amount;
        _playerCurrencyText.text = _playerCurrency.ToString();

        if(amount > 0)
            _playerCurrencyText.GetComponent<RectTransform>().DOScaleY(1.4f, 0.1f).OnComplete(() =>
            {
                _playerCurrencyText.GetComponent<RectTransform>().DOScaleY(1, 0.05f);
            });
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

        if (amount > 0)
            _playerPointsText.GetComponent<RectTransform>().DOScaleY(1.4f, 0.1f).OnComplete(() =>
             {
                 _playerPointsText.GetComponent<RectTransform>().DOScaleY(1, 0.05f);
             });
    }

    public void SubstractPlayerPoints(int amount)
    {
        _playerPoints -= amount;
        _playerPointsText.text = _playerPoints.ToString();
    }

    private void GameStartupConfig()
    {
        StartCoroutine(SetMainGridInitialCards());
        _playerCurrencyText.text = _playerCurrency.ToString(); 
    }

    IEnumerator SetMainGridInitialCards()
    {
        yield return new WaitForSeconds(0.5f);

        //int random = Random.Range(4, 6);
        //for (int i = 0; i < random; i++)
        for (int i = 0; i < 6; i++)
        {
            AssignNewRandomCardToRandomSlot(_spawnPopUpSFX);
            yield return new WaitForSeconds(0.20f);
        }

    }

    public void UpdatePlayerStats(object sender, CombinationData combinationData)
    {
        AddPlayerCurrency(combinationData.currencyValue);
        AddPlayerPoints(combinationData.scoreValue);
        combinedCardsChain++;
        if (combinedCardsChain >= 3)
        {
            AssignNewRandomCardToRandomSlot(_altSpawnPopUpSFX);
            combinedCardsChain = 0;
        };
    }

    private void AssignNewRandomCardToRandomSlot(AudioClip cardSpawnClip)
    {
        CardSlot slot = GetRandomFreeSlot();
        if (slot == default) return;

        CardSO cardData = GetRandomCardSOFromLootPack(_lootPack);
        GameObject card = GenerateCardFromData(cardData);

        slot.Occupy(true);
        PlayOneShotClip(cardSpawnClip);
        card.transform.SetParent(slot.transform);
        card.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);

    }

    private CardSO GetRandomCardSOFromLootPack(LootPackSO lootPack)
    {
        return lootPack.GetRandomItem().item;
    }

    private GameObject GenerateCardFromData(CardSO cardData)
    {
        GameObject card = Instantiate(_cardPrefab, transform.position, Quaternion.identity);
        card.GetComponent<Card>().SetData(cardData);
        card.transform.localScale = Vector3.zero;
        return card;
    }

    public void AssignCardToCardPileFreeSlot(GameObject card)
    {
        foreach(GameObject slot in _cardPileGridSlots)
        {
            var slotComponent = slot.GetComponent<CardSlot>();
            if(!slotComponent.IsOccupied)
            {
                PlayOneShotClip(_spawnPopUpSFX);
                card.GetComponent<Transform>().SetParent(slot.GetComponent<Transform>());
                card.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
                slotComponent.Occupy(true);
                break;
            }
        }
    }

    public bool IsCardPileGridFull()
    {
        return _cardPileGridSlots.Select(x => x.GetComponent<CardSlot>().IsOccupied).All(slot => slot == true);
    }

    public void PlayOneShotClip(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    public void CheckForSpecialCard()
    {
        var randomNumber = Random.Range(0, 100);
        Debug.Log($"Chance to spawn {randomNumber}");
        if (randomNumber < 90) SpawnSpecialCard();
    }

    private void SpawnSpecialCard()
    {
        if (_specialCardSlot.IsOccupied) return;
        PlayOneShotClip(_specialCardSpawnedSFX);
        GameObject goldCard = GenerateCardFromData(_specialCardData);
        goldCard.transform.SetParent(_specialCardSlot.transform);
        goldCard.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        _specialCardSlot.Occupy(true);
    }

    public bool CheckIsSpecialCardRarity(int rarity)
    {
        return rarity == _specialCardData.Rarity;
    }

    public void SpecialCardUse()
    {
        StartCoroutine("SpecialCardAction");
        _specialCardSlot.Occupy(false);
    }

    public IEnumerator SpecialCardAction()
    {
        for (int i = 0; i < 3; i++)
        {
            CardSlot cardSlot = GetRandomOccupiedSlot();
            if (cardSlot != default)
            {
                GameObject cardslotGO = cardSlot.gameObject;
                cardSlot.GetComponent<CardSlot>().Occupy(false);
                GameObject child = cardSlot.transform.GetChild(0).gameObject;
                Destroy(child);
                PlayOneShotClip(_destroyCardSFX);
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
