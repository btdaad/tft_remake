using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BaseItemSO", menuName = "Scriptable Objects/BaseItem")]
public class BaseItemSO : ScriptableObject
{
    public enum Stat
    {
        ATK_DMG,
        ARMOR,
        MAGIC_RESIST,
        PV
    };

    [System.Serializable]
    public struct Modifier
    {
        public Stat stat;
        public float value;
        public bool isFlat;
        public Modifier(Stat stat, float value, bool isFlat)
        {
            this.stat = stat;
            this.value = value;
            this.isFlat = isFlat;
        }
    }

    public string itemName;
    public Texture2D icon;

    [TextArea]
    public string destription;
    [SerializeField] public List<Modifier> modifiers;
}
