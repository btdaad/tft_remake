using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBoardManager
{
    private float MIN_BATTLEFIELD_Z = 0f;
    private float MAX_BATTLEFIELD_Z = 5.25f;
    private BoardManager _boardManager;
    private Vector3 _initUnitPos;
    private Tilemap _battlefieldTilemap;
    private Tilemap _benchTilemap;

    // @param side : can be either "Player" or "Opponent"
    public PlayerBoardManager(string side, BoardManager boardManager)
    {
        _battlefieldTilemap = null;
        _benchTilemap = null;

        _initUnitPos = Vector3.zero;
        Tilemap[] tilemaps = boardManager.gameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag($"{side} Battlefield"))
                _battlefieldTilemap = tilemap;
            else if (tilemap.CompareTag($"{side} Bench"))
                _benchTilemap = tilemap;
        }
        if (_battlefieldTilemap == null
            || _benchTilemap == null)
            Debug.LogError("Could not find every part of the board");

        _boardManager = BoardManager.Instance(_battlefieldTilemap, _benchTilemap);
    }
    public bool OnDragUnit(Transform unitTransform)
    {
        _initUnitPos = unitTransform.position;
        Vector3Int cellPos = _battlefieldTilemap.WorldToCell(_initUnitPos);
        if (_battlefieldTilemap.cellBounds.Contains(cellPos) && _battlefieldTilemap.HasTile(cellPos))
            return true;
        cellPos = _benchTilemap.WorldToCell(_initUnitPos);
        if (_benchTilemap.cellBounds.Contains(cellPos) && _benchTilemap.HasTile(cellPos))
            return true;
        return false;
    }

    public void OnDropUnit(Transform unitTransform)
    {
        if (unitTransform == null)
            return;

        Vector3 unitPos = new Vector3(unitTransform.position.x, _initUnitPos.y, unitTransform.position.z);
        if (!DropOnZone(unitTransform, unitPos, _battlefieldTilemap)) // unit is not dropped on the player battlefield
        {
            if (!DropOnZone(unitTransform, unitPos, _benchTilemap)) // nor on the player bench
                unitTransform.position = _initUnitPos; // restore unit position
        }
    }

    public Vector3 GetCellCenterWorldBattlefield(Vector3Int cellPos)
    {
        return _battlefieldTilemap.GetCellCenterWorld(cellPos);
    }

    private bool DropOnZone(Transform unitTransform, Vector3 unitPos, Tilemap boardZone)
    {
        Vector3Int cellPos = boardZone.WorldToCell(unitPos);

        if (boardZone.cellBounds.Contains(cellPos) && boardZone.HasTile(cellPos))
        {
            Vector3 cellCenterPos = boardZone.GetCellCenterWorld(cellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, _initUnitPos.y, cellCenterPos.z);
            bool successfullyPlaced = PlaceUnitOnZone(unitTransform, cellPos, boardZone);

            if (successfullyPlaced) // can fail if the team size if already maximum
            {
                // propagate info to subscribers (update synergies)
                MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(unitTransform, boardZone == _battlefieldTilemap ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
                _boardManager.CallMoveUnit(null, moveUnitEventArgs);
            }

            return true;
        }
        return false;
    }

    public (int, int) ToBattlefieldCoord(Vector3 position)
    {
        Vector3Int cellPos = _battlefieldTilemap.WorldToCell(position);
        return ToBattlefieldCoord(cellPos);
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

    public bool ToBenchPosition(int index, bool isPlayer, out Vector3 benchPosition)
    {
        Vector3Int cellPos = new Vector3Int(index - 1, isPlayer ? -1 : 8, 0);
        benchPosition = _benchTilemap.GetCellCenterWorld(cellPos);
        bool isCellValid = _benchTilemap.cellBounds.Contains(cellPos) && _benchTilemap.HasTile(cellPos);
        return isCellValid;
    }

    private bool PlaceUnitOnZone(Transform unitTransform, Vector3Int cellPos, Tilemap boardZone)
    {
        // assess init unit zone depending on the z coord
        bool isInitUnitOnBattlefield = _initUnitPos.z >= MIN_BATTLEFIELD_Z && _initUnitPos.z <= MAX_BATTLEFIELD_Z;

        // get cell coords of the init position of the dropped unit, depending on the zone (battlefield of bench)
        Vector3Int initUnitCell = isInitUnitOnBattlefield ? _battlefieldTilemap.WorldToCell(_initUnitPos) : _benchTilemap.WorldToCell(_initUnitPos);
        (int xInitCellPos, int yInitCellPos) = isInitUnitOnBattlefield ? ToBattlefieldCoord(initUnitCell) : ToBenchCoord(initUnitCell);

        if (boardZone == _battlefieldTilemap) // unit is placed on the board
        {
            (int xPos, int yPos) = ToBattlefieldCoord(cellPos); // get grid coordinates for drop cell
            Transform swapUnitTransform = _boardManager.GetUnitAt(xPos, yPos, true); // get the unit on the drop cell

            if (!isInitUnitOnBattlefield && swapUnitTransform == null // add new unit on the battlefield (no swap)
                && _boardManager.IsMaxTeamSize()) // the team size is already maximum
            {
                unitTransform.position = _initUnitPos; // restore unit position
                return false;
            }

            _boardManager.SetUnitAt(xInitCellPos, yInitCellPos, swapUnitTransform, isInitUnitOnBattlefield); // set grid init cell to swap unit
            _boardManager.SetUnitAt(xPos, yPos, unitTransform, true); // set grid drop cell to unit

            if (swapUnitTransform != null)
            {
                swapUnitTransform.position = new Vector3(_initUnitPos.x, swapUnitTransform.position.y, _initUnitPos.z); // if the swap unit exists, move its position

                // propagate info to subscribers (update synergies)
                MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(swapUnitTransform, isInitUnitOnBattlefield ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
                _boardManager.CallMoveUnit(null, moveUnitEventArgs);
            }
        }
        else // unit is placed on the bench
        {
            (int xPos, int yPos) = ToBenchCoord(cellPos);
            Transform swapUnitTransform = _boardManager.GetUnitAt(xPos, yPos, false); // get the unit on the drop cell

            _boardManager.SetUnitAt(xInitCellPos, yInitCellPos, swapUnitTransform, isInitUnitOnBattlefield); // set grid init cell to swap unit
            _boardManager.SetUnitAt(xPos, yPos, unitTransform, false); // set grid drop cell to unit

            if (swapUnitTransform != null)
            {
                swapUnitTransform.position = new Vector3(_initUnitPos.x, swapUnitTransform.position.y, _initUnitPos.z); // if the swap unit exists, move its position

                MoveUnitEventArgs moveUnitEventArgs = new MoveUnitEventArgs(swapUnitTransform, isInitUnitOnBattlefield ? MoveUnitEventArgs.Zone.Battlefield : MoveUnitEventArgs.Zone.Bench);
                _boardManager.CallMoveUnit(null, moveUnitEventArgs);
            }
        }

        return true;
    }

    public bool MoveUnitTo(Transform unitTransform, Vector3Int targetCellPos)
    {
        if (_battlefieldTilemap.cellBounds.Contains(targetCellPos) && _battlefieldTilemap.HasTile(targetCellPos))
        {
            Vector3 cellCenterPos = _battlefieldTilemap.GetCellCenterWorld(targetCellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, unitTransform.position.y, cellCenterPos.z);
            return true;
        }
        return false;
    }
}
