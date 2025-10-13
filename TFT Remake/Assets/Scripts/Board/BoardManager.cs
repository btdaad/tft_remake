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
    private static PathFindingInfo[][] _pathFindingInfo = null;
    public static BoardManager Instance(Tilemap battlefieldTilemap, Tilemap benchTilemap)
    {
        if (_battlefieldGrid == null || _benchGrid == null)
        {
            InitBoard(battlefieldTilemap, ref _battlefieldGrid, false);
            InitBoard(benchTilemap, ref _benchGrid, true);
            InitPathFindingInfo();
        }
        return _instance;
    }

    private PlayerBoardManager _playerBoardManager;
    private PlayerBoardManager _opponentBoardManager;

    public event EventHandler MoveUnit = delegate { };
    [SerializeField] GameObject arrowHelperPrefab;

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

    public bool OnDragUnit(bool isPlayer, Transform unitTransform)
    {
        if (isPlayer)
            return _playerBoardManager.OnDragUnit(unitTransform);
        else
            return _opponentBoardManager.OnDragUnit(unitTransform);
    }
    public void OnDropUnit(bool isPlayer, Transform unitTransform)
    {
        if (isPlayer)
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

    public PathFindingInfo[][] GetDistances()
    {
        return _pathFindingInfo;
    }

    private static void InitBoard(Tilemap tilemap, ref Transform[][] board, bool isBench)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int boardSize = bounds.size + bounds.position;
        board = new Transform[isBench ? 2 : boardSize.y][];
        for (int i = 0; i < board.Length; i++)
            board[i] = new Transform[boardSize.x + 1];
    }
    private static void InitPathFindingInfo()
    {
        int rowsNb = _battlefieldGrid.Length;
        int colsNb = _battlefieldGrid[0].Length;

        _pathFindingInfo = JaggedArrayUtil.InitJaggedArray<PathFindingInfo>(rowsNb, colsNb, () => new PathFindingInfo(rowsNb, colsNb));
        for (int x = 0; x < rowsNb; x++)
            for (int y = 0; y < colsNb; y++)
                _pathFindingInfo[x][y].ComputeDistances(x, y);
    }

    private Vector3Int CoordsToTilemapCell(Coords coords)
    {
        return new Vector3Int(coords.y - 1, coords.x, 0);
    }

    public void DisplayPath(Coords startingCell)
    {
        PathFindingInfo pathFindingInfo = _pathFindingInfo[startingCell.x][startingCell.y];
        PathFindingInfo.HexCellInfo[][] hexCellInfos = pathFindingInfo.GetHexCellInfos();

        for (int x = 0; x < hexCellInfos.Length; x++)
        {
            for (int y = 0; y < hexCellInfos[x].Length; y++)
            {
                if (hexCellInfos[x][y].dist != 0)
                {
                    Coords targetCoords = new Coords(x, y);
                    Vector3Int targetCellPos = CoordsToTilemapCell(targetCoords);
                    Vector3 targetCellCenter = _playerBoardManager.GetCellCenterWorldBattlefield(targetCellPos);

                    Vector3Int fromCellPos = CoordsToTilemapCell(hexCellInfos[x][y].fromCell);
                    Vector3 fromCellCenter = _playerBoardManager.GetCellCenterWorldBattlefield(fromCellPos);

                    Vector3 direction = targetCellCenter - fromCellCenter;
                    Instantiate(arrowHelperPrefab, fromCellCenter, Quaternion.Euler(0, 90, 0) * Quaternion.LookRotation(direction, Vector3.up));
                }
            }
        }
    }
}
