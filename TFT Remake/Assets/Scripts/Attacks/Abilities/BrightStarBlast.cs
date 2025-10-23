using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Syndra's")]
public class BrightStarBlast : AbilityBase 
{
    private int[] damage = { 215, 325, 485 };
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        effects.Add(GetMagicDamage(caster, damage[(int)caster.stats.star]));
        effects.Add(new Effect(-10, EffectType.MAGIC_RESIST));
        return effects;
    }
}