using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Syndra's")]

/* Deal 215/325/485 (AP) magic damage to the target and reduces their Magic Resist by 10. */
public class BrightStarBlast : AbilityBase 
{
    private int[] damageAP = { 215, 325, 485 };
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float damage = ScaleValueWithAP(caster, damageAP[(int)caster.GetStar()]);
        effects.Add(GetMagicDamage(damage));
        effects.Add(new Effect(-10, EffectType.MAGIC_RESIST));

        listEffects.Add(effects);
        return listEffects;
    }
}