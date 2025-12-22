using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public abstract class ItemSO : ScriptableObject
{
    public string itemName;
    public string iconName;
    public Texture2D icon;

    [TextArea]
    public string description;

    public void LoadIconTexture()
    {
        icon = Resources.Load<Texture2D>(iconName);
    }
}
