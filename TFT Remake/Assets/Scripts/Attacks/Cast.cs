using UnityEngine;
using System.Collections.Generic;

public class Cast : MonoBehaviour
{
    private Unit _caster = null;
    private Unit _targetUnit = null;
    private Transform _target = null;
    private List<AbilityBase.Effect> _effects = null;
    [SerializeField] float speed;

    private bool _goThroughEnemies = false;
    private float _timeForDespawn = 1.0f;
    private float _timeSinceSpawn = 0.0f;
    
    void Start()
    { }

    void Update()
    {
        if (_goThroughEnemies)
        {
            if (_timeSinceSpawn >= _timeForDespawn)
                Destroy(this.gameObject);

            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
            _timeSinceSpawn += Time.deltaTime;
        }
        else if (_target != null)
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
    
    private void ApplyEffect(Unit targetUnit, AbilityBase.Effect effect)
    {
        switch (effect.stat)
        {
            case AbilityBase.EffectType.HEALTH:
                _caster.UpdateHealth(effect.damage);
                break;
            case AbilityBase.EffectType.MAGIC_RESIST:
                targetUnit.UpdateMR(effect.damage);
                break;
            case AbilityBase.EffectType.MAGIC_DAMAGE:
                targetUnit.TakeDamage(effect.damage, false);
                break;
            case AbilityBase.EffectType.PHYSICAL_DAMAGE:
                targetUnit.TakeDamage(effect.damage, true);
                break;
            default:
                Debug.LogError($"Ability Effect is not handled for this stat {effect.stat}");
                break;
        }
    }

    public void SetTarget(Unit caster, Unit targetUnit, List<AbilityBase.Effect> effects, bool goThroughEnemies)
    {
        _caster = caster;
        _targetUnit = targetUnit;
        _target = targetUnit.transform;
        _effects = effects;
        _goThroughEnemies = goThroughEnemies;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_goThroughEnemies)
        {
            Unit targetUnit = other.GetComponent<Unit>();
            if (targetUnit != null && targetUnit.IsFromPlayerTeam() != _caster.IsFromPlayerTeam())
            {
                foreach (AbilityBase.Effect effect in _effects)
                    ApplyEffect(targetUnit, effect);
            }
        }
        else if (_target != null && _target.GetComponent<Collider>() == other)
        {
            foreach (AbilityBase.Effect effect in _effects)
                ApplyEffect(_targetUnit, effect);
            Destroy(this.gameObject);
        }
    }
}
