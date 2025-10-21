using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    float _health;
    float _mana;
    float _ap;
    float _ad;
    [SerializeField] bool _isFromPlayerTeam;
    bool _hasMoved;
    float _lastAttack; // time since last attack
    Vector3 _position = Vector3.zero;
    [SerializeField] GameObject _basicAttackPrefab;
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
    }

    // Update is called once per frame
    void Update()
    {
        _lastAttack += Time.deltaTime;
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

    // Returns wether the unit died
    public bool TakeDamage(float damageRaw)
    {
        float r = GetArmor();
        float dm = damageRaw / (1 + (r / 100)); // damage post-mitigation (https://wiki.leagueoflegends.com/en-us/Armor)

        // (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
        if (_mana != stats.mana[1])
        {
            float manaGenerated = 0.01f * damageRaw; // taking damage generates (1% of pre-mitigation damage taken
            manaGenerated += 0.03f * dm; // and 3% of post-mitigation damage taken) mana
            manaGenerated = Mathf.Min(manaGenerated, 42.5f); // up to 42.5 Mana
            _mana += manaGenerated;
            _mana = Mathf.Min(_mana, stats.mana[1]);
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

    private void BasicAttack(Transform opponentTransform)
    {
        Unit opponent = opponentTransform.GetComponent<Unit>();
        float basicAttack = stats.attackDamage[(int)stats.star];

        bool crit = UnityEngine.Random.Range(1, 100) <= GetCritChance();
        basicAttack *= crit ? GetCritDamage() / 100 : 1;

        Vector3 basicAttackPos = new Vector3(transform.position.x, 0.4f, transform.position.z);
        Vector3 dir = opponentTransform.position - transform.position;
        GameObject _basicAttackGO = Instantiate(_basicAttackPrefab, basicAttackPos, Quaternion.LookRotation(dir, Vector3.up));

        if (crit)
            _basicAttackGO.GetComponent<MeshRenderer>().material = Resources.Load("BasicAttackMatCrit", typeof(Material)) as Material;

        _basicAttackGO.GetComponent<Cast>().SetTarget(opponent, basicAttack);

        if (_mana != stats.mana[1])
        {
            _mana += 10.0f; // "All units generate 10 Mana per attack" (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
            _mana = Mathf.Min(_mana, stats.mana[1]);
        }
    }

    private void SpecialAbility(Transform opponentTransform)
    {

    }

    // Returns wether the attacked unit died;
    public void Attack(Transform opponentTransform)
    {
        if (opponentTransform == null) // if unit died during the attack call
            return;

        if (CanAttack())
        {
            if (_mana == stats.mana[1])
                SpecialAbility(opponentTransform);
            else
                BasicAttack(opponentTransform);

            _lastAttack = 0.0f;
        }
    }
}
