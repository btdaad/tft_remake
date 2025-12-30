using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public ItemSO itemSO;
    [SerializeField] public bool isCombinedItem = false;
    [SerializeField] public bool isConsumableItem = false;

    public void Dematerialize(bool destroy)
    {
        if (destroy)
            GameObject.Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public void Materialize()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Texture2D icon = itemSO.icon;
        mat.mainTexture = icon;
        mat.SetFloat("_Smoothness", 0f);

        renderer.material = mat;

        gameObject.SetActive(true);
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

    public bool Use(Item[] items)
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

        // TODO : remove item from the grid ; already done through DragAndDrop but it is done after the removing
        // so, the first empty position found on the item bench does not take the remover cell into account
        GameManager.Instance.GetItemManager().RemoveItems(items, reforgeItems);

        return true;
    }

    public (ItemSO, bool) GetItem()
    {
        return (itemSO, isConsumableItem);
    }

    // TODO : implement passive
}
