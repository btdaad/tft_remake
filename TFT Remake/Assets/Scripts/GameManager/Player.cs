using UnityEngine;

public class Player
{
    int _hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        _hp = 100;
    }

    public void LoseHP(int damage)
    {
        _hp -= damage;
    }

    public int GetHP()
    {
        return _hp;
    }
}
