using UnityEngine;
using UnityEngine.Tilemaps;
using System;

// Vocabulary note :
// the "battlefield" : the game board part where active units are placed, where the fight happens
// the "bench" : the game board part used to store passive units 
// a "zone" : a part of the game board, usually either the battlefield or the bench

// it is expected that a method mentionning "Zone" can handle both the battlefield and the bench
// while "Board" represents the data structure (jagged array) actually used to store information (not the "physical" game board)
// it is expected that a method mentionning "Board" handle both the battlefield and the bench data structures 

public class BoardManager : MonoBehaviour
{
    private static BoardManager _instance;
    private static Transform[][] _battlefieldGrid = null;
    private static Transform[][] _benchGrid = null;
    public static BoardManager Instance(Tilemap battlefieldTilemap, Tilemap benchTilemap)
    {
        if (_battlefieldGrid == null || _benchGrid == null)
        {
            InitBoard(battlefieldTilemap, ref _battlefieldGrid, false); // TODO : use a singleton to not override battlefield twice
            InitBoard(benchTilemap, ref _benchGrid, true); // TODO : same than above
        }
        return _instance;
    }

    private PlayerBoardManager _playerBoardManager;
    private PlayerBoardManager _opponentBoardManager;

    public event EventHandler MoveUnit = delegate {};

    public void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            _instance = this;

        _playerBoardManager = new PlayerBoardManager("Player", this);
        _opponentBoardManager = new PlayerBoardManager("Opponent", this);
        MoveUnit = GameManager.Instance.UpdateSynergies; // add UpdateSynergies to the subscribers
    }

    public void OnDragUnit(string side, Transform unitTransform)
    {
        if (side == "Player")
            _playerBoardManager.OnDragUnit(unitTransform);
        else
            _opponentBoardManager.OnDragUnit(unitTransform);
    }
    public void OnDropUnit(string side, Transform unitTransform)
    {
        if (side == "Player")
            _playerBoardManager.OnDropUnit(unitTransform);
        else
            _opponentBoardManager.OnDropUnit(unitTransform);
    }

    public Transform GetUnitAt(int xPos, int yPos, bool isBattlefield)
    {
        if (isBattlefield)
            return _battlefieldGrid[yPos][xPos];
        else
            return _benchGrid[yPos][xPos];
    }

    public void SetUnitAt(int xPos, int yPos, Transform unitTransform, bool isBattlefield)
    {
        if (isBattlefield)
            _battlefieldGrid[yPos][xPos] = unitTransform;
        else
            _benchGrid[yPos][xPos] = unitTransform;
    }

    public void CallMoveUnit(object sender, MoveUnitEventArgs moveUnitEventArgs)
    {
        MoveUnit(sender, moveUnitEventArgs);
    }

    public Transform[][] GetBattlefield()
    {
        return _battlefieldGrid;
    }

    private static void InitBoard(Tilemap tilemap, ref Transform[][] board, bool isBench)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int boardSize = bounds.size + bounds.position;
        board = new Transform[isBench ? 2 : boardSize.y][];
        for (int i = 0; i < board.Length; i++)
            board[i] = new Transform[boardSize.x + 1];
    }
    public void DumpBoard(Transform[][] board)
    {
        string str = "[";
        foreach (var row in board)
        {
            str += "[";
            int i = 0;
            for (; i < row.Length - 1; i++)
                str += row[i] + ", ";
            str += row[i];
            str += "]\n";
        }
        if (str[str.Length - 1] == '\n')
            str = str.Remove(str.Length - 1);
        Debug.Log(str + "]");
    }
}
