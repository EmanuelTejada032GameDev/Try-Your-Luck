using UnityEngine;

[System.Serializable]
public class LootPackItem
{
     public CardSO item;
    [SerializeField] private float _dropChance;

    public float DropChance()
    {
        return _dropChance;
    }
}
