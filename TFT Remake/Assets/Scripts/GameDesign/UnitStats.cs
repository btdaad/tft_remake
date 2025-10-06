using UnityEngine;

[CreateAssetMenu(fileName = "UnitStat", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public enum Cost
    {
        OneCost = 1,
        TwoCost = 2,
        ThreeCost = 3,
        // FourCost = 4,
        // FiveCost = 5,
        None = 0
    };

    public enum Star
    {
        OneStar = 0,
        TwoStar = 1,
        ThreeStar = 2,
        None = -1
    };


    public Cost cost;
    public Star star;

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
