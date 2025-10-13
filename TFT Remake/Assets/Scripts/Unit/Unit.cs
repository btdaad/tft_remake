using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    float _health;
    float _mana;
    float _ap;
    float _ad;
    [SerializeField] bool _isPlayerTeam;
    bool _hasMoved = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = stats.health[(int)stats.star];
        _mana = stats.mana[0];
        _ap = 0f;
        _ad = 0f;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
            Debug.Log($"{this} has died");
    }

    public void Attack(Transform opponentTransform)
    {
        Debug.Log("Attack");
        Unit opponent = opponentTransform.GetComponent<Unit>();
        opponent.TakeDamage(stats.attackDamage[(int)stats.star]);
    }
}
