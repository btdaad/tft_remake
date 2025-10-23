using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Magic Damage")]
public class MagicDamage : AbilityBase 
{
    private int[] damageAP = { 215, 350, 530 };
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        float magicDamage = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        Effect effect = GetMagicDamage(magicDamage);
        effects.Add(effect);
        return effects;
    }
}