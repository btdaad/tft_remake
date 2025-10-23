using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Physical Damage")]
public class PhysicalDamage : AbilityBase 
{
    private int[] damageAD = { 215, 350, 530 };
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        float physicalDamage = ScaleValueWithAD(caster, damageAD[(int)caster.stats.star]);
        Effect effect = GetPhysicalDamage(physicalDamage);
        effects.Add(effect);
        return effects;
    }
}