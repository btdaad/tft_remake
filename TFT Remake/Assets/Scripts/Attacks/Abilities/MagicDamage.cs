using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Magic Damage")]
public class MagicDamage : AbilityBase 
{
    private int[] damage = { 215, 350, 530 };
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        Effect effect = GetMagicDamage(caster, damage[(int)caster.stats.star]);
        effects.Add(effect);
        return effects;
    }
}