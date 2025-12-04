using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public BaseItemSO baseItemSO;
    [SerializeField] public CombinedItemSO combinedItemSO = null;
    [SerializeField] public bool isCombinedItem = false;

    public void Dematerialize()
    {
        gameObject.SetActive(false);
    }

    public void BecomesCombined(CombinedItemSO combinedItemSO)
    {
        this.baseItemSO = null;
        this.combinedItemSO = combinedItemSO;
        isCombinedItem = true;
    }

    public BaseItemSO GetItem()
    {
        return isCombinedItem ? combinedItemSO : baseItemSO;
    }

    // TODO : implement passive
}
