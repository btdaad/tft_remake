using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    float _health;
    float _mana;
    float _ap;
    float _ad;
    [SerializeField] bool _isFromPlayerTeam;
    bool _hasMoved;
    float _lastAttack; // time since last basic attack
    float _lastAbility; // time since last special ability
    float _manaOverflow;
    Vector3 _position = Vector3.zero;
    [SerializeField] private GameObject _basicAttackPrefab;
    [SerializeField] AbilityBase ability;
    public event EventHandler OnDeath = delegate { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Random.InitState(300600);

        Init();
    }

    void Init()
    {
        _health = stats.health[(int)stats.star];
        _mana = stats.mana[0];
        _ap = 0f;
        _ad = 0f;
        _hasMoved = false;
        _lastAttack = 0.0f;
        _lastAbility = 0.0f;
        _manaOverflow = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _lastAttack += Time.deltaTime;
        _lastAbility += Time.deltaTime;
    }

    public float GetHealth()
    {
        return _health;
    }

    public float GetMana()
    {
        return _mana;
    }

    public float GetAP()
    {
        return _ap;
    }

    public float GetAD()
    {
        return _ad;
    }

    public float GetArmor()
    {
        return stats.armor; // apply modifiers here
    }

    public float GetMagicResist()
    {
        return stats.magicResist; // apply modifiers here
    }

    public float GetAS()
    {
        return stats.attackSpeed; // apply modifiers here
    }

    public float GetCritChance()
    {
        return stats.critChance;
    }

    public int GetRange()
    {
        return stats.range; // apply modifiers here
    }

    private float GetCritDamage()
    {
        return stats.critDamage;
    }

    public bool IsFromPlayerTeam()
    {
        return _isFromPlayerTeam;
    }

    public bool HasMoved()
    {
        return _hasMoved;
    }

    public void SetHasMoved(bool hasMoved)
    {
        _hasMoved = hasMoved;
    }

    public void SavePosition()
    {
        _position = transform.position;
    }

    private void RestorePosition()
    {
        transform.position = _position;
    }

    public void Reset()
    {
        RestorePosition();
        Init();
    }
    
    // After casting their Special Ability, champions can't accumulate mana for the second thereafter. 
    private bool IsManaLocked()
    {
        return _lastAbility < 1f; 
    }

    // Mana that is gained that would overflow will be carried over up to one cast (50/60 + 20 mana = 10/60 mana after casting).
    private void HandleManaOverflow()
    {
        if (_mana > stats.mana[1])
        {
            _manaOverflow += (_mana - stats.mana[1]);
            _mana = stats.mana[1];
        }
    }

    // Returns wether the unit died
    public bool TakeDamage(float damageRaw, bool isPhysicalDamage)
    {
        float r = isPhysicalDamage ? GetArmor() : GetMagicResist();
        float dm = damageRaw / (1 + (r / 100)); // damage post-mitigation (https://wiki.leagueoflegends.com/en-us/Armor)

        // (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
        if (!IsManaLocked() && _mana != stats.mana[1])
        {
            float manaGenerated = 0.01f * damageRaw; // taking damage generates (1% of pre-mitigation damage taken
            manaGenerated += 0.03f * dm; // and 3% of post-mitigation damage taken) mana
            manaGenerated = Mathf.Min(manaGenerated, 42.5f); // up to 42.5 Mana
            _mana += manaGenerated;
            HandleManaOverflow();
        }

        _health -= dm;
        if (_health <= 0)
        {
            _health = 0;
            OnDeathEventArgs onDeathEventArgs = new OnDeathEventArgs(transform);
            OnDeath(null, onDeathEventArgs);
            return true;
        }

        return false;
    }

    private bool CanAttack()
    {
        return _lastAttack >= (1 / GetAS());
    }

    private void CastSphere(Unit opponent, string material, float damage, bool isPhysicalDamage)
    {
        Transform opponentTransform = opponent.transform;

        Vector3 basicAttackPos = new Vector3(transform.position.x, 0.4f, transform.position.z);
        Vector3 dir = opponentTransform.position - transform.position;
        GameObject _basicAttackGO = Instantiate(_basicAttackPrefab, basicAttackPos, Quaternion.LookRotation(dir, Vector3.up));

        _basicAttackGO.tag = "Attack";

        _basicAttackGO.GetComponent<MeshRenderer>().material = Resources.Load(material, typeof(Material)) as Material;

        _basicAttackGO.GetComponent<Cast>().SetTarget(opponent, damage, isPhysicalDamage);
    }

    private void BasicAttack(Transform opponentTransform)
    {
        Unit opponent = opponentTransform.GetComponent<Unit>();
        float basicAttack = stats.attackDamage[(int)stats.star];

        bool crit = UnityEngine.Random.Range(1, 100) <= GetCritChance();
        basicAttack *= crit ? GetCritDamage() / 100 : 1;

        CastSphere(opponent, crit ? "BasicAttackMatCrit" : "BasicAttackMat", basicAttack, true);

        if (!IsManaLocked() && _mana != stats.mana[1])
        {
            _mana += 10.0f; // "All units generate 10 Mana per attack" (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
            HandleManaOverflow();
        }
    }

    private void SpecialAbility(Transform opponentTransform)
    {
        AbilityBase.Effect effect = ability.GetDamage(this);
        List<Unit> targets = ability.targetZone.GetTargets(this);
        if (targets == null) // == target is closest enemy
        {
            Unit opponent = opponentTransform.GetComponent<Unit>();
            CastSphere(opponent, "AbilityMat", effect.damage, effect.isPhysicalDamage);
        }
        else
        {
            foreach (Unit opponent in targets)
                CastSphere(opponent, "AbilityMat", effect.damage, effect.isPhysicalDamage);
        }
        _mana = _manaOverflow;
        _manaOverflow = 0.0f;
    }

    // Returns wether the attacked unit died;
    public void Attack(Transform opponentTransform)
    {
        if (opponentTransform == null) // if unit died during the attack call
            return;

        if (_mana == stats.mana[1]) // TODO : cast directly when mana is full
        {
            SpecialAbility(opponentTransform);
            _lastAbility = 0.0f;
        }

        if (CanAttack())
        {
            BasicAttack(opponentTransform);
            _lastAttack = 0.0f;
        }
    }
}
