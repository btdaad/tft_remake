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
        float damage = ScaleValueWithAP(caster, damageAP[(int)caster.GetStar()]);
        effects.Add(GetMagicDamage(damage));

        listEffects.Add(effects);
        return listEffects;
    }
}