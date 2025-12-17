using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CombinedItemSO", menuName = "Scriptable Objects/CombinedItem")]
public class CombinedItemSO : BaseItemSO
{
    public string item1Name;
    public string item2Name;
    public BaseItemSO item1;
    public BaseItemSO item2;

    public void SetItemCombination(BaseItemSO item1, BaseItemSO item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}
