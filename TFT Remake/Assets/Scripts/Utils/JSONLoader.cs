using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class JSONLoader : MonoBehaviour
{
    [SerializeField] public string baseItemsFilename;
    [SerializeField] public string combinedItemsFilename;
    [SerializeField] public ItemDatabase itemDatabase;
    Dictionary<string, BaseItemSOTest> baseItemDictionary;

    void Start()
    {
        baseItemDictionary = new Dictionary<string, BaseItemSOTest>();
        using (StreamReader r = new StreamReader(baseItemsFilename))
        {
            string json = r.ReadToEnd();
            List<BaseItemSOTest> baseItems = JsonConvert.DeserializeObject<List<BaseItemSOTest>>(json);
            foreach (BaseItemSOTest baseItem in baseItems)
            {
                baseItem.LoadIconTexture();
                baseItemDictionary[baseItem.itemName] = baseItem;
            }
        }

        using (StreamReader r = new StreamReader(combinedItemsFilename))
        {
            string json = r.ReadToEnd();
            List<CombinedItemSOTest> combinedItems = JsonConvert.DeserializeObject<List<CombinedItemSOTest>>(json);
            foreach (CombinedItemSOTest combinedItem in combinedItems)
            {
                combinedItem.LoadIconTexture();
                combinedItem.SetBaseItemSO(baseItemDictionary[combinedItem.item1], baseItemDictionary[combinedItem.item2]);
            }
        
            itemDatabase.SetCombinedItemSOTest(combinedItems);
        }

        // string json = File.ReadAllText(filename);

        // Debug.Log(json);

        // T soObject = ScriptableObject.CreateInstance<T>();
        // JsonUtility.FromJsonOverwrite(json, soObject);

        // Debug.Log(soObject.itemName);
    }
}