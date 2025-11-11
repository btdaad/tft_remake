using UnityEngine;
using UnityEngine.Tilemaps;

public class GoldManager : MonoBehaviour
{
    private int MAX_INTEREST = 5;
    [SerializeField] GameObject[] playerGoldStacks = new GameObject[5];
    [SerializeField] GameObject[] opponentGoldStacks = new GameObject[5];
    public void Init()
    {
        for (int i = 0; i < playerGoldStacks.Length; i++)
        {
            playerGoldStacks[i].SetActive(false);
            opponentGoldStacks[i].SetActive(false);
        }
    }

    private int ComputeInterest(Player player)
    {
        int gold = player.GetGold();
        int interest = (gold * 10) / 100;
        interest = Mathf.Min(interest, MAX_INTEREST);
        return interest;
    }
    // (https://op.gg/fr/tft/game-guide/gold-xp)
    private int ComputePassiveIncome(int mainRound, int subRound)
    {
        if (mainRound >= 2 && subRound >= 2)
            return 5; // 2-2+ Round
        if (mainRound == 1)
        {
            if (subRound == 2 || subRound == 3)
                return 2; // 1-2 and 1-3 Round
            return 3; // 1-4 Round
        }
        return 4; // 2.1 Round
    }

    // (https://wiki.leagueoflegends.com/en-us/TFT:Gold)
    private int ComputeStreakIncome(int streak)
    {
        if (streak < 3)
            return 0;
        if (streak < 5)
            return 1; // when at 3-4 wins in a row.
        if (streak < 6)
            return 2; // when at 5 wins in a row.
        return 3; // when at 6+ wins in a row.
    }

    private int ComputeTotalIncome(Player player)
    {
        int income = ComputeStreakIncome(player.GetWinStreak());
        income += ComputeStreakIncome(player.GetLossStreak());
        income += ComputePassiveIncome(2, 2); // TODO: call actual round manager
        income += ComputeInterest(player);
        return income;
    }

    public void BonusPvP(Player player)
    {
        player.UpdateGold(1);

        GameManager.Instance.UpdateGoldDisplay();
    }

    private void UpdateGoldBank(Player player, GameObject[] goldStacks)
    {
        int nbStack = player.GetGold() / 10;
        for (int i = 0; i < goldStacks.Length; i++)
        {
            int stackIndex = i + 1;
            if (stackIndex <= nbStack)
                goldStacks[i].SetActive(true);
            else
                goldStacks[i].SetActive(false);
        }
    }

    public void ManageGold(Player player, Player opponent)
    {
        int playerIncome = ComputeTotalIncome(player);
        UpdateGold(true, playerIncome);

        int opponentIncome = ComputeTotalIncome(opponent);
        UpdateGold(false, opponentIncome);
    }

    public int GetGold(bool isPlayer)
    {
        return GameManager.Instance.GetPlayer(isPlayer).GetGold();
    }

    public void UpdateGold(bool isPlayer, int amount)
    {
        Player curPlayer = GameManager.Instance.GetPlayer(isPlayer);
        curPlayer.UpdateGold(amount);
        UpdateGoldBank(curPlayer, isPlayer ? playerGoldStacks : opponentGoldStacks);

        GameManager.Instance.UpdateGoldDisplay();
    }
}