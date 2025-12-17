using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CombinedItemSOTest", menuName = "Scriptable Objects/CombinedItemTest")]
public class CombinedItemSOTest : BaseItemSOTest
{
    public string item1;
    public string item2;
    public BaseItemSOTest item1SO;
    public BaseItemSOTest item2SO;
    public void SetBaseItemSO(BaseItemSOTest item1SO, BaseItemSOTest item2SO)
    {
        this.item1SO = item1SO;
        this.item2SO = item2SO;
    }
}
