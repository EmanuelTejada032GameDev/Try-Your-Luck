using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject[] _cardSlots = new GameObject[6];
    [SerializeField] private List<CardSO> _cardsTypes = new List<CardSO>();
    [SerializeField] private GameObject _cardPrefab;


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
    }

    private void GameStartupConfig()
    {
        int counter = 0;
        do
        {
            GameObject randomSlot = _cardSlots[Random.Range(0, _cardSlots.Length)];
            if (!randomSlot.GetComponent<CardSlot>().IsOccupied)
            {
                AssignNewCardToSlot(randomSlot);
                counter++;
            }
        } while (counter < 6);
            
    }

    private void AssignNewCardToSlot(GameObject randomSlot)
    {
        GameObject InstantiatedCardPrefab = Instantiate(_cardPrefab, randomSlot.GetComponent<Transform>().position, Quaternion.identity);
        InstantiatedCardPrefab.GetComponent<Card>().SetData(_cardsTypes[Random.Range(0, _cardsTypes.Count)]);
        InstantiatedCardPrefab.transform.SetParent(randomSlot.transform, false);

        randomSlot.GetComponent<CardSlot>().Occupy(true);
    }

}
