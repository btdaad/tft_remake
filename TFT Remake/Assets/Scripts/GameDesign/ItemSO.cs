using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public abstract class ItemSO : ScriptableObject
{
    public string itemName;
    public Texture2D icon;
}
