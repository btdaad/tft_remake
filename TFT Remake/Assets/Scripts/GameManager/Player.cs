using UnityEngine;

public class Player
{
    int _hp;
    int _gold;
    int _winStreak;
    int _lossStreak;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        _hp = 100;
        _gold = 0;
        _winStreak = 0;
        _lossStreak = 0;
    }
    public int GetHP()
    {
        return _hp;
    }

    public int GetGold()
    {
        return _gold;
    }

    public int GetWinStreak()
    {
        return _winStreak;   
    }

    public int GetLossStreak()
    {
        return _lossStreak;
    }

    public void Defeat(int damage)
    {
        _hp -= damage;
        _winStreak = 0;
        _lossStreak++;
    }

    public void Victory()
    {
        _lossStreak = 0;
        _winStreak++;
    }

    public void UpdateGold(int amount)
    {
        _gold += amount;
    }
}
