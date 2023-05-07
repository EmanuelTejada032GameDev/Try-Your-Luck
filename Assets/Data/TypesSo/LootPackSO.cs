using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LootPack", fileName = "LootPack")]
public class LootPackSO : ScriptableObject
{
    [SerializeField] private List<LootPackItem> _possibleDrops;

    private float _lootPackTotalWeight;
    public bool _isTotalWeigthSet = false;

    private void SetTotalWeigth()
    {
        _lootPackTotalWeight = _possibleDrops.Sum(x => x.DropChance());
        _isTotalWeigthSet = true;
    }

    public LootPackItem GetRandomItem()
    {
        SetTotalWeigth();
        var rollDice = Random.Range(0, _lootPackTotalWeight);
        foreach (var item in _possibleDrops)
        {
            float itemDropChance = item.DropChance();
            if (itemDropChance > rollDice)
                return item;
            else
                rollDice -= itemDropChance;
        }
        return default;
    }


    public List<LootPackItem> GetRandomItems(int itemsAmount)
    {
        var items = new List<LootPackItem>();
        for (int i = 0; i < itemsAmount; i++)
        {
            items.Add(GetRandomItem());
        }
        return items ;  
    }
}


