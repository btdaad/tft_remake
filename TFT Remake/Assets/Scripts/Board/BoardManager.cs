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
    private float MIN_BATTLEFIELD_Z = 0f;
    private float MAX_BATTLEFIELD_Z = 5.25f;
    private GameManager _gameManager;
    Vector3 _initUnitPos;
    Tilemap _playerBattlefield;
    Tilemap _playerBench;

    Transform[][] _battlefield;
    Transform[][] _bench;

    public event EventHandler MoveUnit = delegate {};

    public void Init()
    {
        _playerBattlefield = null;
        _playerBench = null;

        _initUnitPos = Vector3.zero;
        Tilemap[] tilemaps = gameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag("Player Battlefield"))
                _playerBattlefield = tilemap;
            else if (tilemap.CompareTag("Player Bench"))
                _playerBench = tilemap;
        }

        InitBoard(_playerBattlefield, ref _battlefield, false);
        InitBoard(_playerBench, ref _bench, true);

        _gameManager = GameManager.Instance;
        MoveUnit = _gameManager.UpdateSynergies; // add UpdateSynergies to the subscribers
    }

    public void OnDragUnit(Transform unitTransform)
    {
        _initUnitPos = unitTransform.position;
    }

    public void OnDropUnit(Transform unitTransform)
    {
        if (unitTransform == null)
            return;

        Vector3 unitPos = new Vector3(unitTransform.position.x, _initUnitPos.y, unitTransform.position.z);
        if (!DropOnZone(unitTransform, unitPos, _playerBattlefield))
        {
            if (!DropOnZone(unitTransform, unitPos, _playerBench))
                unitTransform.position = _initUnitPos;
        }
    }

    private bool DropOnZone(Transform unitTransform, Vector3 unitPos, Tilemap boardZone)
    {
        Vector3Int cellPos = boardZone.WorldToCell(unitPos);

        if (boardZone.cellBounds.Contains(cellPos) && boardZone.HasTile(cellPos))
        {
            Vector3 cellCenterPos = boardZone.GetCellCenterWorld(cellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, _initUnitPos.y, cellCenterPos.z);
            PlaceUnitOnZone(unitTransform, cellPos, boardZone);
            
            // propagate info to subscribers (update synergies)
            MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(unitTransform, boardZone == _playerBattlefield ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
            MoveUnit(null, moveUnitEventArgs);

            return true;
        }
        return false;
    }

    public Transform[][] GetBattlefield()
    {
        return _battlefield;
    }

    // Careful : the battlefield cell width coords go from -1 to 6 so the index on the x axis are incremented by 1
    private (int, int) ToBattlefieldCoord(Vector3Int cellCoord)
    {
        return (cellCoord.x + 1, cellCoord.y);
    }

    // Careful : the bench cell width coords go from -1 to 8 so the index on the x axis are incremented by 1
    // also, on the y axis it goes from -1 to 9 but all rows between 0 and 8 are useless so the y coordinates are either 0 or 1 to match the array index
    private (int, int) ToBenchCoord(Vector3Int cellCoord)
    {
        return (cellCoord.x + 1, cellCoord.y == -1 ? 0 : 1);
    }

    private void PlaceUnitOnZone(Transform unitTransform, Vector3Int cellPos, Tilemap boardZone)
    {
        // assess init unit zone depending on the z coord
        bool isInitUnitOnBattlefield = _initUnitPos.z >= MIN_BATTLEFIELD_Z && _initUnitPos.z <= MAX_BATTLEFIELD_Z;

        // get cell coords of the init position of the dropped unit, depending on the zone (battlefield of bench)
        Vector3Int initUnitCell = isInitUnitOnBattlefield ? _playerBattlefield.WorldToCell(_initUnitPos) : _playerBench.WorldToCell(_initUnitPos);
        (int xInitCellPos, int yInitCellPos) = isInitUnitOnBattlefield ? ToBattlefieldCoord(initUnitCell) : ToBenchCoord(initUnitCell);

        if (boardZone == _playerBattlefield)
        {
            (int xPos, int yPos) = ToBattlefieldCoord(cellPos); // get grid coordinates for drop cell
            Transform swapUnitTransform = _battlefield[yPos][xPos]; // get the unit on the drop cell

            // set grid init cell to swap unit
            if (isInitUnitOnBattlefield)
                _battlefield[yInitCellPos][xInitCellPos] = swapUnitTransform;
            else
                _bench[yInitCellPos][xInitCellPos] = swapUnitTransform;

            _battlefield[yPos][xPos] = unitTransform; // set grid drop cell to unit

            if (swapUnitTransform != null)
            {
                swapUnitTransform.position = _initUnitPos; // if the swap unit exists, move its position

                // propagate info to subscribers (update synergies)
                MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(swapUnitTransform, isInitUnitOnBattlefield ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
                MoveUnit(null, moveUnitEventArgs);
            }
        }
        else
        {
            (int xPos, int yPos) = ToBenchCoord(cellPos);
            Transform swapUnitTransform = _bench[yPos][xPos];

            if (isInitUnitOnBattlefield)
                _battlefield[yInitCellPos][xInitCellPos] = swapUnitTransform;
            else
                _bench[yInitCellPos][xInitCellPos] = swapUnitTransform;

            _bench[yPos][xPos] = unitTransform;

            if (swapUnitTransform != null)
            {
                swapUnitTransform.position = _initUnitPos;

                MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(swapUnitTransform, isInitUnitOnBattlefield ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
                MoveUnit(null, moveUnitEventArgs);
            }
        }
    }

    private void InitBoard(Tilemap tilemap, ref Transform[][] board, bool isBench)
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
