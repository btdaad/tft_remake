using UnityEngine;
using UnityEngine.Tilemaps;

public class GoldManager : MonoBehaviour
{
    private int MAX_INTEREST = 5;
    [SerializeField] GameObject goldStack;
    private Tilemap _playerGoldTilemap; // {-2, 1|2|3|4|5, 0}
    private (int, int)[] _playerGoldCoords = { (-2, 1), (-2, 2), (-2, 3), (-2, 4), (-2, 5) };
    private Tilemap _opponentGoldTilemap; // {-3, 0|1|2|3|4, 0}
    private (int, int)[] _opponentGoldCoords = { (-3, 0), (-3, 1), (-3, 2), (-3, 3), (-3, 4) };
    public void Init()
    {
        _playerGoldTilemap = null;
        _opponentGoldTilemap = null;

        Tilemap[] tilemaps = GameManager.Instance.GetBoardManager().gameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag($"Player Gold"))
                _playerGoldTilemap = tilemap;
            else if (tilemap.CompareTag($"Opponent Gold"))
                _opponentGoldTilemap = tilemap;
        }
        if (_playerGoldTilemap == null
            || _opponentGoldTilemap == null)
            Debug.LogError("Could not find every gold bank");
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

    private void ComputeTotalIncome(Player player)
    {
        int income = ComputeStreakIncome(player.GetWinStreak());
        income += ComputeStreakIncome(player.GetLossStreak());
        income += ComputePassiveIncome(2, 2); // TODO: call actual round manager
        income += ComputeInterest(player);
        player.UpdateGold(income);
    }

    public void BonusPvP(Player player)
    {
        player.UpdateGold(1);
    }

    private void UpdateGoldBank(Player player, Tilemap goldBankTilemap, (int, int)[] goldBankCoords)
    {
        int stack = player.GetGold() / 10;
        stack = Mathf.Min(stack, goldBankCoords.Length);
        for (int i = 0; i < stack; i++)
        {
            (int x, int y) = goldBankCoords[i];
            Vector3 position = goldBankTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
            Instantiate(goldStack, position, Quaternion.identity);
            // TODO : remove stack not used
        }
    }

    public void ManageGold(Player player, Player opponent)
    {
        ComputeTotalIncome(player);
        UpdateGoldBank(player, _playerGoldTilemap, _playerGoldCoords);
        ComputeTotalIncome(opponent);
        UpdateGoldBank(opponent, _opponentGoldTilemap, _opponentGoldCoords);
    }
}
