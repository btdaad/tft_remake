using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;            

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    [SerializeField] public float scaleFactor = 1.14f;
    Star _star = Star.OneStar;
    float _health;
    float _mana;
    float _ap;
    float _ad;
    float _mr;
    List<Shield> _shields;
    List<Shield> _durability;
    [SerializeField] bool isFromPlayerTeam;
    bool _hasMoved;
    bool _isStun;
    float _lastAttack; // time since last basic attack
    float _lastAbility; // time since last special ability
    float _manaOverflow;
    Vector3 _position = Vector3.zero;
    [SerializeField] private GameObject _basicAttackPrefab;
    [SerializeField] AbilityBase ability;
    public event EventHandler OnDeath = delegate { };
    
    [SerializeField] private Item[] _items = new Item[3]{null, null, null};
    private float _armor_modifier;
    private float _mr_modifier;
    private float _pv_modifier;
    private ItemDatabase _itemDatabase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Random.InitState(300600);

        Init();
    }

    void Init()
    {
        _health = stats.health[(int)_star];
        _mana = stats.mana[0];
        _mr = stats.magicResist;
        _shields = new List<Shield>();
        _durability = new List<Shield>();
        _hasMoved = false;
        _lastAttack = 0.0f;
        _lastAbility = 0.0f;
        _manaOverflow = 0.0f;
        
        _itemDatabase = (ItemDatabase) GameObject.FindFirstObjectByType (typeof(ItemDatabase));
        if (_itemDatabase == null)
            Debug.Log("Could not find the item database.");
        _pv_modifier = 0.0f;
        UpdateModifiers();
    }

    // Update is called once per frame
    void Update()
    {
        _lastAttack += Time.deltaTime;
        _lastAbility += Time.deltaTime;

        for (int i = 0; i < _shields.Count; i++)
        {
            if (_shields[i].duration > 0.0f) // if time is < 0 it means the shield does not expirate
            {
                _shields[i].duration -= Time.deltaTime;
                _shields[i].duration = Mathf.Max(_shields[i].duration, 0.0f);
            }
        }
        _shields.RemoveAll(shield => shield.strength <= 0.0f || shield.duration == 0.0f);

        for (int i = 0; i < _durability.Count; i++)
        {
            if (_durability[i].duration > 0.0f) // if time is < 0 it means the shield does not expirate
            {
                _durability[i].duration -= Time.deltaTime;
                _durability[i].duration = Mathf.Max(_durability[i].duration, 0.0f);
            }
        }
        _durability.RemoveAll(durability => durability.strength <= 0.0f || durability.duration == 0.0f);
    }

    public void UpLevel()
    {
        _star = (Star) (int)_star + 1;
        _health = stats.health[(int)_star];
        transform.localScale = transform.localScale * scaleFactor;
    }

    public void SetAffiliation(bool isPlayer)
    {
        isFromPlayerTeam = isPlayer;
    }

    private void UpdateModifiers()
    {
        _ad = 0.0f;
        _ap = 0.0f;
        _armor_modifier = 0.0f;
        _mr_modifier = 0.0f;

        UpdateHealth(-_pv_modifier);

        int i = 0;
        while (i < _items.Length && _items[i] != null)
        {
            BaseItemSO baseItemSO = _items[i].GetItem();
            foreach (BaseItemSO.Modifier modifier in baseItemSO.modifiers)
            {
                switch (modifier.stat)
                {
                    case BaseItemSO.Stat.ATK_DMG:
                        if (modifier.isFlat)
                            _ad += modifier.value;
                        else
                            Debug.Log("Do something");
                        break;
                    case BaseItemSO.Stat.ARMOR:
                        if (modifier.isFlat)
                            _armor_modifier += modifier.value;
                        else
                            Debug.Log("Do something");
                        break;
                    case BaseItemSO.Stat.MAGIC_RESIST:
                        if (modifier.isFlat)
                            _mr_modifier += modifier.value;
                        else
                            Debug.Log("Do something");
                        break;
                    case BaseItemSO.Stat.PV:
                        if (modifier.isFlat)
                            _pv_modifier += modifier.value;
                        else
                            Debug.Log("Do something");
                        break;
                    default:
                        Debug.Log("Stat not handled yet");
                        break;
                }
            }

            i++;
        }

        UpdateHealth(_pv_modifier);
    }

    public bool SetItem(Item newItem)
    {
        int i = 0;
        while (i < _items.Length)
        {
            if (!newItem.isCombinedItem
                && _items[i] != null
                && !_items[i].isCombinedItem)
            {
                CombinedItemSO combinedItemSO = _itemDatabase.GetCombined(_items[i].baseItemSO, newItem.baseItemSO);
                if (combinedItemSO == null)
                {
                    Debug.Log("These items does not combine");
                    return false;
                }
                _items[i].BecomesCombined(combinedItemSO);
                break;
            }
            else if (_items[i] == null) // implies either newItem is combined or not, it does not matter
            {
                _items[i] = newItem;
                break;
            }
            i++;
        }

        if (i == _items.Length)
            return false; // item was not given to unit

        newItem.Dematerialize();

        UpdateModifiers();

        return true;
    }

    public Item[] GetItems()
    {
        return _items;
    }

    public Star GetStar()
    {
        return _star;
    }

    public float GetHealth()
    {
        return _health;
    }

    // Returns the rest of the damage that the shields couldn't take, as a POSITIVE number
    // @params float damage : a positive number interpreted as damage to the shield 
    private float ShieldDamageAux(float damage)
    {
        while (damage > 0.0f && _shields.Count > 0)
        {
            if (_shields[0].strength >= damage)
            {
                _shields[0].strength -= damage;
                damage = 0.0f;
            }
            else
            {
                damage -= _shields[0].strength;
                _shields.RemoveAt(0);
            }
        }
        return damage;
    }

    // Wrapper around the actual shield damage handling in order to turn damage into positive values
    private float ShieldDamage(float damage)
    {
        return -ShieldDamageAux(-damage);
    }

    private IEnumerator UpdateHealthCoroutine(float value, float duration)
    {
        float timeElapsed = 0.0f;
        float healthPerSec = value / duration;
        float deltaTime = 0.2f;

        while (timeElapsed < duration)
        {
            float deltaHealth = healthPerSec * deltaTime;
            UpdateHealth(deltaHealth);
            yield return new WaitForSeconds(deltaTime);
            timeElapsed += deltaTime;
        }
        yield return null;
    }

    public void UpdateHealth(float value, float duration = 0f)
    {
        if (duration == 0f) // instantaneous
        {
            if (value < 0.0f) // damage are taken by the shields
            {
                value = ShieldDamage(value);
                // TODO : implement magic and physical shield
                if (value == 0.0f)
                    return;
            }

            _health += value;
            if (_health <= 0.0f)
            {
                _health = 0;
                OnDeathEventArgs onDeathEventArgs = new OnDeathEventArgs(transform);
                OnDeath(null, onDeathEventArgs);
            }
        }
        else
            StartCoroutine(UpdateHealthCoroutine(value, duration));
    }

    public float GetShield()
    {
        float totalShieldStrength = 0.0f;
        foreach (Shield shield in _shields)
            totalShieldStrength += shield.strength;
        return totalShieldStrength;
    }

    public void SetShield(float strength, float duration)
    {
        _shields.Add(new Shield(strength, duration));
    }

    public float GetDurability()
    {
        float totalDurability = 1.0f;
        foreach (Shield durability in _durability)
            totalDurability *= (1.0f - durability.strength);
        return 1.0f - totalDurability;
    }

    public void GainDurability(float strength, float duration)
    {
        _durability.Add(new Shield(strength, duration));
    }

    public float GetMaxHealth()
    {
        return stats.health[(int)GetStar()] + _pv_modifier; // apply modifiers
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

    public float GetMR()
    {
        return _mr + _mr_modifier;
    }

    public void UpdateMR(float value)
    {
        _mr += value;
        _mr = Mathf.Max(_mr, 0.0f);
    }

    public float GetArmor()
    {
        return stats.armor + _armor_modifier;
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

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isStun = false;
    }

    public void Stun(float duration)
    {
        _isStun = true;
        StartCoroutine(StunCoroutine(duration));
    }

    public bool IsStun()
    {
        return _isStun;
    }

    public bool IsFromPlayerTeam()
    {
        return isFromPlayerTeam;
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

    private IEnumerator TakeDamageCoroutine(float damageRaw, bool isPhysicalDamage, float duration)
    {
        float timeElapsed = 0.0f;
        float damagePerSec = damageRaw / duration;
        float deltaTime = 0.2f;

        while (timeElapsed < duration)
        {
            float deltaDamage = damagePerSec * deltaTime;
            TakeDamage(damagePerSec, isPhysicalDamage);
            yield return new WaitForSeconds(deltaTime);
            timeElapsed += deltaTime;
        }
        yield return null;
    }

    public void TakeDamage(float damageRaw, bool isPhysicalDamage, float duration = 0f)
    {
        if (duration == 0f) // instantaneous
        {
            float r = isPhysicalDamage ? GetArmor() : GetMR();
            float dm = damageRaw / (1 + (r / 100)); // damage post-mitigation (https://wiki.leagueoflegends.com/en-us/Armor)

            dm *= (1.0f - GetDurability());

            // (https://leagueoflegends.fandom.com/wiki/Mana_(Teamfight_Tactics))
            if (!IsManaLocked() && _mana != stats.mana[1])
            {
                float manaGenerated = 0.01f * damageRaw; // taking damage generates (1% of pre-mitigation damage taken
                manaGenerated += 0.07f * dm; // and 7% of post-mitigation damage taken) mana
                manaGenerated = Mathf.Min(manaGenerated, 42.5f); // up to 42.5 Mana
                _mana += manaGenerated;
                HandleManaOverflow();
            }

            UpdateHealth(-dm); // TODO : magic/pysical shield, add here the isPhysicalDamage info
        }
        else
            StartCoroutine(TakeDamageCoroutine(damageRaw, isPhysicalDamage, duration));
    }

    private bool CanAttack()
    {
        return _lastAttack >= (1 / GetAS());
    }

    private void CastSphere(Unit opponent, string material, List<AbilityBase.Effect> effects, bool goThroughEnemies = false)
    {
        Transform opponentTransform = opponent.transform;

        Vector3 basicAttackPos = new Vector3(transform.position.x, 0.4f, transform.position.z);
        Vector3 dir = opponentTransform.position - transform.position;
        GameObject _basicAttackGO = Instantiate(_basicAttackPrefab, basicAttackPos, Quaternion.LookRotation(dir, Vector3.up));

        _basicAttackGO.tag = "Attack";

        _basicAttackGO.GetComponent<MeshRenderer>().material = Resources.Load(material, typeof(Material)) as Material;

        _basicAttackGO.GetComponent<Cast>().SetTarget(this, opponent, effects, goThroughEnemies);
    }

    private void BasicAttack(Transform opponentTransform)
    {
        Unit opponent = opponentTransform.GetComponent<Unit>();
        float basicAttack = stats.attackDamage[(int)GetStar()];

        bool crit = UnityEngine.Random.Range(1, 100) <= GetCritChance();
        basicAttack *= crit ? GetCritDamage() / 100 : 1;

        List<AbilityBase.Effect> effects = new List<AbilityBase.Effect>();
        effects.Add(new AbilityBase.Effect(basicAttack, AbilityBase.EffectType.PHYSICAL_DAMAGE));
        CastSphere(opponent, crit ? "BasicAttackMatCrit" : "BasicAttackMat", effects);

        if (!IsManaLocked() && _mana != stats.mana[1])
        {
            _mana += 10.0f; // "All units generate 10 Mana per attack" (https://wiki.leagueoflegends.com/en-us/TFT:Mana)
            HandleManaOverflow();
        }
    }

    private void SpecialAbility(Transform opponentTransform)
    {
        List<List<AbilityBase.Effect>> effects = ability.GetEffects(this);
        for (int i = 0; i < effects.Count; i++)
        {
            ability.targetZones[i].SetTarget(opponentTransform);
            List<Unit> targets = ability.targetZones[i].GetTargets(this);

            if (ability.targetZones[i] is Line) // only one sphere is cast, but it goes **through** the enemies
                CastSphere(targets[0], "AbilityMat", effects[i], true);
            else
            {
                foreach (Unit opponent in targets)
                    CastSphere(opponent, "AbilityMat", effects[i]);
            }
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
