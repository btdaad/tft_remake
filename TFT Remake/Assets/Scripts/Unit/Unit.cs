using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    float _health;
    float _mana;
    float _ap;
    float _ad;
    [SerializeField] bool _isPlayerTeam;
    bool _hasMoved;
    float _lastAttack; // time since last attack
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Random.InitState(300600);

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
        return _isPlayerTeam;
    }

    public bool HasMoved()
    {
        return _hasMoved;
    }

    public void SetHasMoved(bool hasMoved)
    {
        _hasMoved = hasMoved;
    }

    public void TakeDamage(float damageRaw)
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
            Debug.Log($"{this} has died");
    }

    private bool CanAttack()
    {
        return _lastAttack >= (1 / GetAS());
    }

    public void Attack(Transform opponentTransform)
    {
        if (CanAttack())
        {
            Unit opponent = opponentTransform.GetComponent<Unit>();
            float basicAttack = stats.attackDamage[(int)stats.star];

            basicAttack *= (Random.Range(1, 100) <= GetCritChance()) ? GetCritDamage() / 100 : 1;

            if (_mana != stats.mana[1])
            {
                _mana += 10.0f; // "All units generate 10 Mana per attack" (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
                _mana = Mathf.Min(_mana, stats.mana[1]);
            }

            opponent.TakeDamage(basicAttack);
            _lastAttack = 0.0f;
        }
    }
}
