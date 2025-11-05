using UnityEngine;
using UnityEngine.Tilemaps;

public class XPManager : MonoBehaviour
{
    private int[] XPperLevel = { 0, 0, 2, 6, 10, 20, 36, 48, 72, 84 };
    [SerializeField] public int XP_COST = 4;
    public void Init()
    {
    }

    public int GetXPCost()
    {
        return XP_COST;
    }

    public int GetXP(bool isPlayer)
    {
        return GameManager.Instance.GetPlayer(isPlayer).GetXP();
    }

    public int GetTotalXP(bool isPlayer)
    {
        int curLevel = GameManager.Instance.GetPlayer(isPlayer).GetLevel();
        if (curLevel != XPperLevel.Length)
            return XPperLevel[curLevel];
        return 0; // TODO : should not be displayed at all !!!
    }
    public int GetLevel(bool isPlayer)
    {
        return GameManager.Instance.GetPlayer(isPlayer).GetLevel();
    }

    public void PassLevel(Player player)
    {
        int curLevel = player.GetLevel();
        if (curLevel != XPperLevel.Length)
        {
            int xpForNextLevel = XPperLevel[curLevel];
            if (player.GetXP() >= xpForNextLevel)
            {
                player.GainLevel();
                player.UpdateXP(-xpForNextLevel);

                GameManager.Instance.UpdateLevelDisplay();
            }
        }
    }

    public void BuyXP(Player player)
    {
        if (player.GetGold() >= XP_COST)
        {
            player.UpdateXP(4);
            player.UpdateGold(-XP_COST);

            PassLevel(player);
            GameManager.Instance.UpdateXPDisplay();
        }
    }
}