using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Yone's")]

/* Passive: Attacks grant 5/5/25% stacking Attack Speed and Movement Speed,
   and alternate between dealing 25/40/400 (AD) bonus true damage and
   100/160/999 (AP) bonus magic damage. // TODO

   Active: Send an echo towards the two furthest enemies, dealing 140/210/9999 (AD)
   physical damage and briefly Stunning enemies hit.*/
public class DualBladeTechnique_InnerReflection : AbilityBase
{
    private int[] damageAD = { 140, 210, 9999 };
    private float stunTime = 1f;
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();

        float damage = ScaleValueWithAD(caster, damageAD[(int)caster.GetStar()]);
        effects.Add(GetPhysicalDamage(damage));

        effects.Add(new Effect(0f, EffectType.STUN, stunTime));

        listEffects.Add(effects);

        return listEffects;
    }
}