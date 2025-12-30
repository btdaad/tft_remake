using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class ItemDatabase
{
    private Dictionary<string, BaseItemSO> _baseItemDictionary;
    public List<CombinedItemSO> combinedItems;
    public List<ConsumableItemSO> consumableItems;
    private Dictionary<(BaseItemSO, BaseItemSO), CombinedItemSO> _itemCombinations;
    public ItemDatabase(string baseItemsFilename, string combinedItemsFilename, string consumableItemsFilename)
    {
        _baseItemDictionary = new Dictionary<string, BaseItemSO>();
        LoadBaseItemsJSONFile(baseItemsFilename);

        LoadCombinedItemsJSONFile(combinedItemsFilename);

        LoadConsumableItemsJSONFile(consumableItemsFilename);

        _itemCombinations = new Dictionary<(BaseItemSO, BaseItemSO), CombinedItemSO>();
        ComputeItemCombinations();
    }

    private void LoadBaseItemsJSONFile(string filename)
    {
        using (StreamReader r = new StreamReader(filename))
        {
            string json = r.ReadToEnd();
            List<BaseItemSO> baseItems = JsonConvert.DeserializeObject<List<BaseItemSO>>(json);
            foreach (BaseItemSO baseItem in baseItems)
            {
                baseItem.LoadIconTexture();
                _baseItemDictionary[baseItem.itemName] = baseItem;
            }
        }
    }

    private void LoadCombinedItemsJSONFile(string filename)
    {
        using (StreamReader r = new StreamReader(filename))
        {
            string json = r.ReadToEnd();
            combinedItems = JsonConvert.DeserializeObject<List<CombinedItemSO>>(json);
            foreach (CombinedItemSO combinedItem in combinedItems)
            {
                combinedItem.LoadIconTexture();
                combinedItem.SetItemCombination(_baseItemDictionary[combinedItem.item1Name], _baseItemDictionary[combinedItem.item2Name]);
            }
        }
    }

    private void LoadConsumableItemsJSONFile(string filename)
    {
        using (StreamReader r = new StreamReader(filename))
        {
            string json = r.ReadToEnd();
            consumableItems = JsonConvert.DeserializeObject<List<ConsumableItemSO>>(json);
            foreach (ConsumableItemSO consumableItem in consumableItems)
                consumableItem.LoadIconTexture();
        }
    }

    private void ComputeItemCombinations()
    {
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

    public bool FindCombination(BaseItemSO item1, BaseItemSO item2, out CombinedItemSO combinedItemSO)
    {
        return _itemCombinations.TryGetValue((item1, item2), out combinedItemSO);
    }

    public bool BaseItemsContains(string itemName)
    {
        return _baseItemDictionary.ContainsKey(itemName);
    }

    public BaseItemSO GetBaseItem(string itemName)
    {
        return _baseItemDictionary[itemName];
    }
    public BaseItemSO GetRandomBaseItem(string itemName)
    {
        System.Random rand = new System.Random();

        List<BaseItemSO> baseItemList = Enumerable.ToList(_baseItemDictionary.Values);
        int size = _baseItemDictionary.Count;
        BaseItemSO baseItemSO;
        do
        {
            baseItemSO = baseItemList[rand.Next(size)];
        } while (String.Compare(baseItemSO.itemName, itemName) == 0);
        return baseItemSO;
    }
    public BaseItemSO GetRandomCombinedItem(string itemName)
    {
        System.Random rand = new System.Random();

        int size = combinedItems.Count;
        CombinedItemSO combinedItemSO;
        do
        {
            combinedItemSO = combinedItems[rand.Next(size)];
        } while (String.Compare(combinedItemSO.itemName, itemName) == 0);
        return (BaseItemSO) combinedItemSO;
    }

    public ConsumableItemSO GetConsumableItem(ConsumableItemSO.ConsumableType type)
    {
        return consumableItems.Find(item => item.type == type);
    }
}