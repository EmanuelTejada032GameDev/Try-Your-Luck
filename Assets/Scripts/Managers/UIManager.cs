using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _cardSlots = new GameObject[6];
    [SerializeField] private List<CardSO> _cardsTypes = new List<CardSO>();
    [SerializeField] private GameObject _cardPrefab;

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
                GameObject InstantiatedCardPrefab = Instantiate(_cardPrefab, randomSlot.GetComponent<Transform>().position, Quaternion.identity);
                InstantiatedCardPrefab.GetComponent<Card>().SetData(_cardsTypes[Random.Range(0, _cardsTypes.Count)]);
                InstantiatedCardPrefab.transform.SetParent(randomSlot.transform, false);

                randomSlot.GetComponent<CardSlot>().Occupy(true);
                counter++;
            }
        } while (counter < 4);
            
    }
}
