using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitStats stats;
    float _health;
    float _mana;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = stats.health[(int)stats.star];
        _mana = 0f;
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
}
