using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] public List<CombinedItemSO> combinedItems;
    private Dictionary<(BaseItemSO, BaseItemSO), CombinedItemSO> _itemCombinations;
    void Awake()
    {
        _itemCombinations = new Dictionary<(BaseItemSO, BaseItemSO), CombinedItemSO>();

        foreach (CombinedItemSO combinedItemSO in combinedItems)
        {
            BaseItemSO item1 = combinedItemSO.item1;
            BaseItemSO item2 = combinedItemSO.item2;

            (BaseItemSO, BaseItemSO) key1 = (item1, item2);
            (BaseItemSO, BaseItemSO) key2 = (item2, item1);

            _itemCombinations[key1] = combinedItemSO;
            _itemCombinations[key2] = combinedItemSO;
        }
    }
    
    public CombinedItemSO GetCombined(BaseItemSO item1, BaseItemSO item2)
    {
        if (_itemCombinations.TryGetValue((item1, item2), out var combinedItemSO))
            return combinedItemSO;

        return null;
    }
}