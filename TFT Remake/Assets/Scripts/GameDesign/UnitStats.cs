using UnityEngine;

[CreateAssetMenu(fileName = "UnitStat", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public enum Cost
    {
        OneCost = 0,
        ThreeCost = 1,
        FiveCost = 2,
        None
    };

    public UnitType type;
    public Cost cost;

    public Trait[] traits = new Trait[3];

    [Header("Stats")]
    public float[] health = new float[3];
    public float[] mana = new float[2];
    public float[] attackDamage = new float[3];
    public float abilityPower;
    public float armor;
    public float magicResist;
    public float attackSpeed;
    [Tooltip("in %")]
    public float critChance;
    [Tooltip("in %")]
    public float critDamage;
    public int range;
}
