using UnityEngine;

[CreateAssetMenu(fileName = "UnitTraitSO", menuName = "Scriptable Objects/UnitTraitSO")]
public class UnitTraitSO : ScriptableObject
{
    public Trait trait;
    public int nbStages;
    public int[] stages;
}
