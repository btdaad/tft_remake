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

    public (ItemSO, bool, bool) GetItem()
    {
        return (itemSO, isCombinedItem, isConsumableItem);
    }

    // TODO : implement passive
}
