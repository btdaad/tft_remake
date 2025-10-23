using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Syndra's")]

/* Deal 215/325/485 (AP) magic damage to the target and reduces their Magic Resist by 10. */
public class BrightStarBlast : AbilityBase 
{
    private int[] damageAP = { 215, 325, 485 };
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        float damage = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        effects.Add(GetMagicDamage(damage));
        effects.Add(new Effect(-10, EffectType.MAGIC_RESIST));
        return effects;
    }
}