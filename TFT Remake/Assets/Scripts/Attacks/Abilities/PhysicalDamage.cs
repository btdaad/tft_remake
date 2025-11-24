using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Physical Damage")]
public class PhysicalDamage : AbilityBase 
{
    private int[] damageAD = { 215, 350, 530 };
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float damage = ScaleValueWithAD(caster, damageAD[(int)caster.GetStar()]);
        effects.Add(GetPhysicalDamage(damage));

        listEffects.Add(effects);
        return listEffects;
    }
}