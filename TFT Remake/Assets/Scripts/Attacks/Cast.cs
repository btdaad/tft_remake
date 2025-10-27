using UnityEngine;
using System.Collections.Generic;

public class Cast : MonoBehaviour
{
    private Unit _caster = null;
    private Unit _targetUnit = null;
    private Transform _target = null;
    private List<AbilityBase.Effect> _effects = null;
    [SerializeField] float speed;
    
    // the following properties are used for casting along a line
    private bool _canGoThroughEnemies = false;
    private float _timeSinceLaunch = 0.0f;
    [SerializeField] float timeBeforeDespawn = 0.0f; // used only if the sphere can go through multiple enemies
    void Start()
    { }

    void Update()
    {
        if (_canGoThroughEnemies) // only set to true when SetTarget is called AND if it is not a basic attack
        {
            if (_timeSinceLaunch >= timeBeforeDespawn)
                Destroy(this.gameObject);
            float step = speed * Time.deltaTime;
            transform.Translate(0.0f, 0.0f, step); // move in target direction (set in SetTarget) ; does not depend on target alive or not
            _timeSinceLaunch += Time.deltaTime;
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

    bool IsBasicAttack()
    {
        return GetComponent<Renderer>().material.name.Contains("BasicAttack");
    }

    public void SetTarget(Unit caster, Unit targetUnit, List<AbilityBase.Effect> effects)
    {
        _caster = caster;
        _targetUnit = targetUnit;
        _target = targetUnit.transform;
        _effects = effects;

        if (!IsBasicAttack() && timeBeforeDespawn != 0.0f)
        {
            _canGoThroughEnemies = true;
            transform.LookAt(_target); // face target
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target != null && _target.GetComponent<Collider>() == other)
        {
            foreach (AbilityBase.Effect effect in _effects)
                ApplyEffect(effect);
            if (IsBasicAttack() || !_canGoThroughEnemies)
                Destroy(this.gameObject);
        }
    }
}
