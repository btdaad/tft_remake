using UnityEngine;
using System.Collections.Generic;

public class Cast : MonoBehaviour
{
    private Unit _caster = null;
    private Unit _targetUnit = null;
    private Transform _target = null;
    private List<AbilityBase.Effect> _effects = null;
    [SerializeField] float speed;
    
    void Start()
    { }

    void Update()
    {
        if (_target != null)
        {
            if (_target.gameObject.activeSelf)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
            }
            else
                Destroy(this.gameObject);
        }
    }
    
    private void ApplyEffect(AbilityBase.Effect effect)
    {
        switch (effect.stat)
        {
            case AbilityBase.EffectType.HEALTH:
                _caster.UpdateHealth(effect.damage);
                break;
            case AbilityBase.EffectType.MAGIC_RESIST:
                _targetUnit.UpdateMR(effect.damage);
                break;
            case AbilityBase.EffectType.MAGIC_DAMAGE:
                _targetUnit.TakeDamage(effect.damage, false);
                break;
            case AbilityBase.EffectType.PHYSICAL_DAMAGE:
                _targetUnit.TakeDamage(effect.damage, true);
                break;
            default:
                Debug.LogError($"Ability Effect is not handled for this stat {effect.stat}");
                break;
        }
    }

    public void SetTarget(Unit caster, Unit targetUnit, List<AbilityBase.Effect> effects)
    {
        _caster = caster;
        _targetUnit = targetUnit;
        _target = targetUnit.transform;
        _effects = effects;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target != null && _target.GetComponent<Collider>() == other)
        {
            foreach (AbilityBase.Effect effect in _effects)
                ApplyEffect(effect);
            Destroy(this.gameObject);
        }
    }
}
