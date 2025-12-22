using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] public string baseItemsFilename;
    [SerializeField] public string combinedItemsFilename;
    [SerializeField] public string consumableItemsFilename;
    private Dictionary<string, BaseItemSO> _baseItemDictionary;
    public List<CombinedItemSO> combinedItems;
    public List<ConsumableItemSO> consumableItems;
    private Dictionary<(BaseItemSO, BaseItemSO), CombinedItemSO> _itemCombinations;
    [SerializeField] public GameObject itemPrefab;
    void LoadJSONFile()
    {
        _baseItemDictionary = new Dictionary<string, BaseItemSO>();
        using (StreamReader r = new StreamReader(baseItemsFilename))
        {
            string json = r.ReadToEnd();
            List<BaseItemSO> baseItems = JsonConvert.DeserializeObject<List<BaseItemSO>>(json);
            foreach (BaseItemSO baseItem in baseItems)
            {
                baseItem.LoadIconTexture();
                _baseItemDictionary[baseItem.itemName] = baseItem;
            }
        }

        using (StreamReader r = new StreamReader(combinedItemsFilename))
        {
            string json = r.ReadToEnd();
            combinedItems = JsonConvert.DeserializeObject<List<CombinedItemSO>>(json);
            foreach (CombinedItemSO combinedItem in combinedItems)
            {
                combinedItem.LoadIconTexture();
                combinedItem.SetItemCombination(_baseItemDictionary[combinedItem.item1Name], _baseItemDictionary[combinedItem.item2Name]);
            }
        }
        
        using (StreamReader r = new StreamReader(consumableItemsFilename))
        {
            string json = r.ReadToEnd();
            consumableItems = JsonConvert.DeserializeObject<List<ConsumableItemSO>>(json);
            foreach (ConsumableItemSO consumableItem in consumableItems)
                consumableItem.LoadIconTexture();
        }
    }

    void Awake()
    {
        LoadJSONFile();
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

    public void CreateItem(string itemName)
    {
        if (_baseItemDictionary.ContainsKey(itemName))
        {
            GameObject itemGO = Instantiate(itemPrefab, new Vector3(-1.63f, 0.23f, -1.34f), itemPrefab.transform.rotation);
            itemGO.layer = LayerMask.NameToLayer("Item");

            Item item = itemGO.GetComponent<Item>();
            item.itemSO = _baseItemDictionary[itemName];

            Renderer renderer = itemGO.GetComponent<Renderer>();

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Texture2D icon = item.itemSO.icon;
            mat.mainTexture = icon;
            mat.SetFloat("_Smoothness", 0f);

            renderer.material = mat;
        }
        else if (itemName == "Reforger" || itemName == "Remover")
        {
            ConsumableItemSO consumableItemSO = consumableItems.Find(item => item.type == ConsumableItemSO.ConsumableType.REMOVER);
            if (itemName == "Reforger")
                consumableItemSO = consumableItems.Find(item => item.type == ConsumableItemSO.ConsumableType.REFORGER);

            GameObject itemGO = Instantiate(itemPrefab, new Vector3(-1.63f, 0.23f, -1.34f), itemPrefab.transform.rotation);
            itemGO.layer = LayerMask.NameToLayer("Item");

            Item item = itemGO.GetComponent<Item>();
            item.itemSO = consumableItemSO;
            item.isConsumableItem = true;

            Renderer renderer = itemGO.GetComponent<Renderer>();

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Texture2D icon = item.itemSO.icon;
            mat.mainTexture = icon;
            mat.SetFloat("_Smoothness", 0f);

            renderer.material = mat;
        }
    }
}