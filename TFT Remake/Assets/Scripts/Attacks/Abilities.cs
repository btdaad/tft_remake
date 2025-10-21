using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Magic Damage")]
public class MagicDamage : AbilityBase 
{
    private int[] damage = { 215, 350, 530 };
    public override void Cast(Unit caster)
    {
    } 
}
