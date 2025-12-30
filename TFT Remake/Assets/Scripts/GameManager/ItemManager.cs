using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public string baseItemsFilename;
    [SerializeField] public string combinedItemsFilename;
    [SerializeField] public string consumableItemsFilename;
    [SerializeField] public GameObject itemPrefab;
    private ItemDatabase _itemDatabase = null;
    public void Init()
    {
        _itemDatabase = new ItemDatabase(baseItemsFilename, combinedItemsFilename, consumableItemsFilename);
    }
    public CombinedItemSO GetCombined(BaseItemSO item1, BaseItemSO item2)
    {
        CombinedItemSO combinedItemSO;
        bool successful = _itemDatabase.FindCombination(item1, item2, out combinedItemSO);
        if (successful)
            return combinedItemSO;
        return null;
    }

#nullable enable
    public Transform? CreateItem(string itemName)
    {
        if (_itemDatabase.BaseItemsContains(itemName))
            return CreateItem(_itemDatabase.GetBaseItem(itemName), false); // new Vector3(-1.63f, 0.23f, -1.34f)); 
        else if (itemName == "Reforger" || itemName == "Remover")
        {
            ConsumableItemSO consumableItemSO = _itemDatabase.GetConsumableItem(ConsumableItemSO.ConsumableType.REMOVER);
            if (itemName == "Reforger")
                consumableItemSO = _itemDatabase.GetConsumableItem(ConsumableItemSO.ConsumableType.REFORGER);

            return CreateItem(consumableItemSO, true);
            // CreateItem(consumableItemSO, true, new Vector3(-1.63f, 0.23f, -1.34f));
        }
        return null;
    }

    private Transform? CreateItem(ItemSO itemSO, bool isConsumableItem)
    {
        GameObject itemGO = Instantiate(itemPrefab, Vector3.zero, itemPrefab.transform.rotation);
        itemGO.layer = LayerMask.NameToLayer("Item");

        Item item = itemGO.GetComponent<Item>();
        item.itemSO = itemSO;
        item.isConsumableItem = isConsumableItem;

        Renderer renderer = itemGO.GetComponent<Renderer>();

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Texture2D icon = item.itemSO.icon;
        mat.mainTexture = icon;
        mat.SetFloat("_Smoothness", 0f);

        renderer.material = mat;

        return itemGO.transform;
    }
#nullable disable

    private void Reforge(Item item)
    {
        if (item.isConsumableItem)
            Debug.LogError("A Consumable item cannot be reforged.");
        else if (item.isCombinedItem)
            item.itemSO = _itemDatabase.GetRandomCombinedItem(item.itemSO.itemName);
        else // base item
            item.itemSO = _itemDatabase.GetRandomBaseItem(item.itemSO.itemName);
    }

    public void RemoveItems(Item[] items, bool reforgeItems)
    {
        for (int i = 0; i < items.Length && items[i] != null; i++)
        {
            Item item = items[i];
            bool successful = GameManager.Instance.GetItemBenchEmptySpot(out Vector3 benchPosition);
            if (successful)
            {
                if (reforgeItems)
                    Reforge(item);
                item.Materialize();
                GameManager.Instance.PlaceItemAt(item.gameObject.transform, benchPosition);
            }
            items[i] = null;
        }
    }
}