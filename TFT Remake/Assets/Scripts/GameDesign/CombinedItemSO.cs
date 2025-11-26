using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CombinedItemSO", menuName = "Scriptable Objects/CombinedItem")]
public class CombinedItemSO : BaseItemSO
{
    public BaseItemSO item1;
    public BaseItemSO item2;
}
