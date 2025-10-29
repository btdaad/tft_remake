using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Magic Damage")]
public class MagicDamage : AbilityBase 
{
    private int[] damageAP = { 215, 350, 530 };
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float magicDamage = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        Effect effect = GetMagicDamage(magicDamage);
        effects.Add(effect);

        listEffects.Add(effects);
        return listEffects;
    }
}