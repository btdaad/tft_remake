using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BaseItemSOTest", menuName = "Scriptable Objects/BaseItemTest")]
public class BaseItemSOTest : ScriptableObject
{
    public enum Stat
    {
        AD, // OK // Attack Damage
        ARMOR, // OK
        MAGIC_RESIST, // OK
        HEALTH, // OK
        DA, // TODO // Damage Amplification
        AS, // OK // Attack Speed
        AP, // OK // Ability Power
        OMNIVAMP, // TODO // https://wiki.leagueoflegends.com/en-us/Vamp // is is omnivamp or spell vamp ?
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
    public string icon;
    public Texture2D iconTexture;

    [TextArea]
    public string description;
    [SerializeField] public List<Modifier> modifiers;

    public void LoadIconTexture()
    {
        iconTexture = Resources.Load<Texture2D>(icon);
    }
}
