using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BaseItemSO", menuName = "Scriptable Objects/BaseItem")]
public class BaseItemSO : ItemSO 
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

    [SerializeField] public List<Modifier> modifiers;
}
