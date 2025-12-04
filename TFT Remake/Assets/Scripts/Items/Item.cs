using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public BaseItemSO baseItemSO;
    [SerializeField] public CombinedItemSO combinedItemSO = null;
    [SerializeField] public bool isCombinedItem = false;

    public void Dematerialize()
    {
        gameObject.SetActive(false);
        // this.GetComponent<MeshRenderer>().enabled = false;
        // this.GetComponent<Collider>().enabled = false;
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
}
