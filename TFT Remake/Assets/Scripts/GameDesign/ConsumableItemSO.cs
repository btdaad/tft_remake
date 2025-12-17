using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConsumableItemSO", menuName = "Scriptable Objects/BaseItem")]
public class ConsumableItemSO : ItemSO 
{
    public enum ConsumableType
    {
        REMOVER,
        REFORGER
    };

    [SerializeField] public ConsumableType type;
}
