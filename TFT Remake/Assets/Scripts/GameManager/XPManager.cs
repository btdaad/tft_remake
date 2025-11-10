using UnityEngine;
using UnityEngine.Tilemaps;

public class XPManager : MonoBehaviour
{
    private int[] XPperLevel = { 0, 2, 2, 6, 10, 20, 36, 48, 72, 84 };
    [SerializeField] public int xpCost = 4;
    [SerializeField] public int endOfRoundXP = 2;

    public void Init()
    {
    }

    public int GetMaxLevel()
    {
        return XPperLevel.Length;
    }

    public int GetXPCost()
    {
        return xpCost;
    }

    public int GetXP(bool isPlayer)
    {
        return GameManager.Instance.GetPlayer(isPlayer).GetXP();
    }

    public int GetTotalXP(bool isPlayer)
    {
        int curLevel = GameManager.Instance.GetPlayer(isPlayer).GetLevel();
        if (curLevel != GetMaxLevel())
            return XPperLevel[curLevel];
        return 0; // TODO : should not be displayed at all !!!
    }
    public int GetLevel(bool isPlayer)
    {
        return GameManager.Instance.GetPlayer(isPlayer).GetLevel();
    }

    public void EndFight(Player player, Player opponent)
    {
        player.UpdateXP(endOfRoundXP);
        opponent.UpdateXP(endOfRoundXP);

        PassLevel(player);
        PassLevel(opponent);
    }

    public void PassLevel(Player player)
    {
        int curLevel = player.GetLevel();
        if (curLevel != GetMaxLevel())
        {
            int xpForNextLevel = XPperLevel[curLevel];
            if (player.GetXP() >= xpForNextLevel)
            {
                player.GainLevel();
                player.UpdateXP(-xpForNextLevel);

                GameManager.Instance.UpdateLevelDisplay();
                PassLevel(player); // maybe there is an overflow of XP so the player can pass multiple levels at once
            }
        }
        GameManager.Instance.UpdateXPDisplay();
    }

    public void BuyXP(Player player)
    {
        if (player.GetGold() >= xpCost)
        {
            player.UpdateXP(4);
            player.UpdateGold(-xpCost);
            GameManager.Instance.UpdateGoldDisplay();

            PassLevel(player);
        }
    }
}