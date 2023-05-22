using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

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
    [SerializeField] private int _playerCurrency = 0;
    [SerializeField] private int _playerPoints = 0;
    [SerializeField] private TextMeshProUGUI _playerCurrencyText;
    [SerializeField] private TextMeshProUGUI _playerPointsText;

    private int _commonPacksBougth = 0;
    private int _plusPacksBougth = 0;
    private int _currencySpent = 0;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _spawnPopUpSFX;
    [SerializeField] private AudioClip _altSpawnPopUpSFX;
    [SerializeField] private AudioClip _destroyCardSFX;

    [SerializeField] private AudioSource _canvasAudioSource;

    public int AvailableCardPileSlots => _cardPileGridSlots.Where(x => x.GetComponent<CardSlot>().IsOccupied == false).Count();
    private int combinedCardsChain = 0;
    public int PlayerCurrency { get => _playerCurrency; }
    public int PlayerScore { get => _playerPoints; }
    public int CurrencySpent { get => _currencySpent; }
    public int CommonPacksBougths { get => _commonPacksBougth; }
    public int PlusPacksBougths { get => _plusPacksBougth; }


    private void Awake()
    {
        Time.timeScale = 0f;
        _canvasAudioSource.Stop();
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public List<CardSO> CardsTypes => _cardsTypes;

    private void Start()
    {
        _playerCurrency = CardPile.Instance.CommonPackPrice * 2;
        _audioSource = GetComponent<AudioSource>();
        Draggable.OnCardCombined += UpdatePlayerStats;
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

        if(_playerCurrencyText != default)
        {
            _playerCurrencyText.text = _playerCurrency.ToString();

            if (amount > 0)
                _playerCurrencyText.GetComponent<RectTransform>().DOScaleY(1.4f, 0.1f).OnComplete(() =>
                {
                    _playerCurrencyText.GetComponent<RectTransform>().DOScaleY(1, 0.05f);
                });
        }
    }

    public void SubstractPlayerCurrency(int amount)
    {
        _playerCurrency -= amount;
        _playerCurrencyText.text = _playerCurrency.ToString();
    }

    public void AddPlayerPoints(int amount)
    {
        _playerPoints += amount;

        if(_playerPointsText != default)
        {
            _playerPointsText.text = _playerPoints.ToString();
            if (amount > 0)
                _playerPointsText.GetComponent<RectTransform>().DOScaleY(1.4f, 0.1f).OnComplete(() =>
                {
                    _playerPointsText.GetComponent<RectTransform>().DOScaleY(1, 0.05f);
                });
        }

       
    }

    public void SubstractPlayerPoints(int amount)
    {
        _playerPoints -= amount;
        _playerPointsText.text = _playerPoints.ToString();
    }

    public void GameStartupConfig()
    {
        StartCoroutine(SetMainGridInitialCards());
        GameLoopMusic(true);
        _playerCurrencyText.text = _playerCurrency.ToString(); 
    }

    IEnumerator SetMainGridInitialCards()
    {
        yield return new WaitForSeconds(0.5f);
        //int random = Random.Range(4, 6);
        //for (int i = 0; i < random; i++)
        for (int i = 0; i < 3; i++)
        {
            AssignNewRandomCardToRandomSlot(_spawnPopUpSFX);
            yield return new WaitForSeconds(0.20f);
        }

    }

    public void AddToSpentCurrency(int amount)
    {
        _currencySpent += amount;
    }

    public void AddPlusPack()
    {
        _plusPacksBougth += 1;
    }

    public void AddCommonPack()
    {
        _commonPacksBougth += 1;
    }

    public void UpdatePlayerStats(object sender, CombinationData combinationData)
    {
        AddPlayerCurrency(combinationData.currencyValue);
        AddPlayerPoints(combinationData.scoreValue);
        combinedCardsChain++;
        if (combinedCardsChain >= 2)
        {
            AssignNewRandomCardToRandomSlot(_altSpawnPopUpSFX);
            combinedCardsChain = 0;
        };

        CheckGameOver();
    }

    public void CheckGameOver()
    {
        var isLostGame = CheckLostGame();
        if (isLostGame)
        {
            Menus.Instance.GameOver();
        }
    }

    public bool CheckLostGame()
    {
        bool enoughCurrency = CheckEnoughCurrency();
        bool checkFreeSlots = CheckFreeSlots();
        bool combinableCardsInGrids = CheckCombinableCardsInGrids();
        bool hasSpecialCard = _specialCardSlot.IsOccupied;

        return (!enoughCurrency && !combinableCardsInGrids && !hasSpecialCard) || (!checkFreeSlots && !combinableCardsInGrids && !hasSpecialCard);
    }

    private bool CheckFreeSlots()
    {
        return (_cardSlots.Select(x => x.GetComponent<CardSlot>().IsOccupied).Any(occupiedSlot => occupiedSlot == false)
                || _cardPileGridSlots.Select(x => x.GetComponent<CardSlot>().IsOccupied).Any(occupiedSlot => occupiedSlot == false));
    }

    private bool CheckCombinableCardsInGrids()
    {
        List<int> cardRaritys = new List<int>();
        foreach (GameObject slot in _cardSlots)
        {
            if (slot.transform.childCount > 0)
            {
                int cardRarity = slot.transform.GetChild(0).GetComponent<Card>().Rarity;
                cardRaritys.Add(cardRarity);
            }
        }

        foreach (GameObject slot in _cardPileGridSlots)
        {
            if (slot.transform.childCount > 0)
            {
                int cardRarity = slot.transform.GetChild(0).GetComponent<Card>().Rarity;
                cardRaritys.Add(cardRarity);
            }
        }

        var count = cardRaritys.Count;
        var distinct = cardRaritys.Distinct().Count();

        return cardRaritys.Count != cardRaritys.Distinct().Count();
    }

    private bool CheckEnoughCurrency()
    {
        return _playerCurrency >= CardPile.Instance.CommonPackPrice;
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
        if (randomNumber < 40) SpawnSpecialCard();
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
        AddPlayerCurrency(CardPile.Instance.CommonPackPrice);
        AddPlayerPoints(250);
        _specialCardSlot.Occupy(false);
    }

    public IEnumerator SpecialCardAction()
    {
        for (int i = 0; i < 2; i++)
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

    public void ResetGame()
    {
        _commonPacksBougth = 0;
        _plusPacksBougth = 0;
        _currencySpent = 0;
        SubstractPlayerCurrency(_playerCurrency);
        SubstractPlayerPoints(_playerPoints);
        AddPlayerCurrency(CardPile.Instance.CommonPackPrice * 2);
        CleanGrids(_cardSlots);
        CleanGrids(_cardPileGridSlots);
        CleanGrids(new GameObject[1] { _specialCardSlot.gameObject });
        GameLoopMusic(false);
    }

    public void PauseGameLoopMusic(bool pause)
    {
        if(pause)
            _canvasAudioSource.Pause();
        else
            _canvasAudioSource.UnPause();
    }

    public void GameLoopMusic(bool turnOn)
    {
        if (turnOn)
        {
            _canvasAudioSource.Play();
        }
        else
        {
            _canvasAudioSource.Stop();
            _canvasAudioSource.time = 0;
        }
    }


    private void CleanGrids(GameObject[] slots)
    {
        foreach (GameObject slot in slots)
        {
            if(slot.transform.childCount > 0)
            {
                slot.GetComponent<CardSlot>().Occupy(false);
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }
}
