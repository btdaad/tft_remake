using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Magic Damage")]
public class MagicDamage : AbilityBase 
{
    private int[] damage = { 215, 350, 530 };
    public override Effect GetDamage(Unit caster)
    {
        float magicDamage = damage[(int)caster.stats.star];
        magicDamage = magicDamage * (caster.stats.abilityPower + caster.GetAD()) / 100f;
        return new Effect(magicDamage, false);
    }
}
