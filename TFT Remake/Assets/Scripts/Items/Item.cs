using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public ItemSO itemSO;
    [SerializeField] public bool isCombinedItem = false;
    [SerializeField] public bool isConsumableItem = false;

    public void Dematerialize()
    {
        gameObject.SetActive(false);
    }

    public void BecomesCombined(CombinedItemSO combinedItemSO)
    {
        if (isConsumableItem)
        {
            Debug.LogError("A consumable item cannot be combined.");
            return;
        }

        this.itemSO = combinedItemSO;
        isCombinedItem = true;
    }

    public bool ApplyEffects(Item[] items)
    {
        if (!isConsumableItem)
        {
            Debug.LogError("Trying to apply effet from a basic item.");
            return false;
        }

        if (items[0] == null // no item to remove or reforge
            || GameManager.Instance.IsFightOngoing()) // cannot reforge or remove items if the fight has started 
            return false;

        bool reforgeItems = false;
        if ((itemSO as ConsumableItemSO).type == ConsumableItemSO.ConsumableType.REFORGER)
            reforgeItems = true;

        GameManager.Instance.GetItemManager().RemoveItems(items, reforgeItems);

        return true;
    }

    public (ItemSO, bool) GetItem()
    {
        return (itemSO, isConsumableItem);
    }

    // TODO : implement passive
}
