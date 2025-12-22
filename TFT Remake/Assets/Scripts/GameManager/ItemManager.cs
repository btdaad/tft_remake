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
    public void CreateItem(string itemName)
    {
        if (_itemDatabase.BaseItemsContains(itemName))
            CreateItem(_itemDatabase.GetBaseItem(itemName), false);
        else if (itemName == "Reforger" || itemName == "Remover")
        {
            ConsumableItemSO consumableItemSO = _itemDatabase.GetConsumableItem(ConsumableItemSO.ConsumableType.REMOVER);
            if (itemName == "Reforger")
                consumableItemSO = _itemDatabase.GetConsumableItem(ConsumableItemSO.ConsumableType.REFORGER);

            CreateItem(consumableItemSO, true);
        }
    }

    private void CreateItem(ItemSO itemSO, bool isConsumableItem)
    {
        GameObject itemGO = Instantiate(itemPrefab, new Vector3(-1.63f, 0.23f, -1.34f), itemPrefab.transform.rotation);
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
    }

    public void RemoveItems(Item[] items, bool reforgeItems)
    {
        GameManager.Instance.GetItemBenchEmptySpot(out Vector3 benchPosition);
        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];


            items[i] = null;
        }
    }
}