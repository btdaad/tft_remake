using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class ItemBenchManager
{
    private BoardManager _boardManager;
    private Vector3 _initItemPos;
    private Tilemap _itemTilemap; // Bounds : (-4, -2, 0) to (14, 5, 1)
    

    // BEGIN TEST ZONE
    private List<Vector3Int> _validTiles;
    private HashSet<Vector3Int> _occupiedTiles;
    // END TEST ZONE

    // @param side : can be either "Player" or "Opponent"
    public ItemBenchManager(string side, BoardManager boardManager)
    {
        _itemTilemap = null;

        _initItemPos = Vector3.zero;
        Tilemap[] tilemaps = boardManager.gameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag($"{side} Item"))
                _itemTilemap = tilemap;
        }
        if (_itemTilemap == null)
            Debug.LogError("Could not find every the item board");

        _boardManager = BoardManager.GetInstanceAndInit(_itemTilemap);


        // BEGIN TEST ZONE
        _validTiles = new List<Vector3Int>();

        BoundsInt bounds = _itemTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (_itemTilemap.HasTile(pos))
                _validTiles.Add(pos);
        }

        _occupiedTiles = new HashSet<Vector3Int>();
        // END TEST ZONE
    }

    // BEGIN TEST ZONE
    public Vector3Int? GetFirstEmptyTile(Material mat, GameObject prefab)
    {
        foreach (Vector3Int cell in _validTiles)
        {
            if (!_occupiedTiles.Contains(cell))
            {
                Vector3 place = _itemTilemap.CellToWorld(cell);
                GameObject ballGO = GameObject.Instantiate(prefab, place, Quaternion.identity) as GameObject;
                ballGO.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                ballGO.GetComponent<Renderer>().material = mat;
                return cell;
            }
        }
        return null;
    }

    public void RemoveItemAt(Vector3Int cellPos)
    {
        _occupiedTiles.Remove(cellPos);
    }

    // END TEST ZONE

    public Vector3 GetInitItemPos()
    {
        return _initItemPos;
    }

    public Vector3Int GetInitItemCellPos()
    {
        return _itemTilemap.WorldToCell(_initItemPos);
    }
    
    public bool OnDragItem(Transform itemTransform)
    {
        _initItemPos = itemTransform.position;
        Vector3Int cellPos = _itemTilemap.WorldToCell(_initItemPos);
        if (_itemTilemap.cellBounds.Contains(cellPos) && _itemTilemap.HasTile(cellPos))
            return true;
        return false;
    }

    public void OnDropItem(Transform itemTransform)
    {
        if (itemTransform == null)
            return;

        Vector3 itemPos = new Vector3(itemTransform.position.x, _initItemPos.y, itemTransform.position.z);
        if (!DropOnZone(itemTransform, itemPos, _itemTilemap)) // item is not dropped on the item board
            itemTransform.position = _initItemPos; // restore item position
    }

    public Vector3 GetCellCenterWorldItemBench(Vector3Int cellPos)
    {
        return _itemTilemap.GetCellCenterWorld(cellPos);
    }

    private bool DropOnZone(Transform itemTransform, Vector3 unitPos, Tilemap boardZone)
    {
        Vector3Int cellPos = boardZone.WorldToCell(unitPos);

        if (boardZone.cellBounds.Contains(cellPos) && boardZone.HasTile(cellPos))
        {
            Vector3 cellCenterPos = boardZone.GetCellCenterWorld(cellPos);
            itemTransform.position = new Vector3(cellCenterPos.x, _initItemPos.y, cellCenterPos.z);
            PlaceItemOnZone(itemTransform, cellPos);
            return true;
        }
        return false;
    }

    public (int, int) ToItemCoord(Vector3 position)
    {
        Vector3Int cellPos = _itemTilemap.WorldToCell(position);
        return ToItemCoord(cellPos);
    }

    private (int, int) ToItemCoord(Vector3Int cellCoord)
    {
        return (cellCoord.x + 5, cellCoord.y + 2); // empirically found values
    }

    private void PlaceItemOnZone(Transform itemTransform, Vector3Int cellPos)
    {
        // get cell coords of the init position of the dropped item
        Vector3Int initUnitCell = _itemTilemap.WorldToCell(_initItemPos);
        (int xInitCellPos, int yInitCellPos) = ToItemCoord(initUnitCell);

        (int xPos, int yPos) = ToItemCoord(cellPos); // get grid coordinates for drop cell
        Transform swapItemTransform = _boardManager.GetItemAt(cellPos); // get the item on the drop cell

        _boardManager.SetItemAt(initUnitCell, swapItemTransform);
        _boardManager.SetItemAt(cellPos, itemTransform);
        _occupiedTiles.Add(cellPos);

        if (swapItemTransform != null)
        {
            swapItemTransform.position = new Vector3(_initItemPos.x, swapItemTransform.position.y, _initItemPos.z); // if the swap item exists, move its position
            _occupiedTiles.Add(initUnitCell);
        }
        else
            _occupiedTiles.Remove(initUnitCell);

        string str = "";
        foreach (Vector3Int v3i in _occupiedTiles)
            str += v3i + " ";
        Debug.Log($"{str}");
    }

    /*
    private void PlaceItemOnZone(Transform itemTransform, Vector3Int cellPos)
    {
        // get cell coords of the init position of the dropped item
        Vector3Int initUnitCell = _itemTilemap.WorldToCell(_initItemPos);
        (int xInitCellPos, int yInitCellPos) = ToItemCoord(initUnitCell);

        (int xPos, int yPos) = ToItemCoord(cellPos); // get grid coordinates for drop cell
        Transform swapItemTransform = _boardManager.GetItemAt(xPos, yPos); // get the item on the drop cell

        _boardManager.SetItemAt(xInitCellPos, yInitCellPos, swapItemTransform); // set grid init cell to swap item
        _boardManager.SetItemAt(xPos, yPos, itemTransform); // set grid drop cell to item 

        if (swapItemTransform != null)
            swapItemTransform.position = new Vector3(_initItemPos.x, swapItemTransform.position.y, _initItemPos.z); // if the swap item exists, move its position
    }*/

    public void Dump(Material inMat, Material outMat, GameObject prefab)
    {
        List<Vector3> availablePlaces = new List<Vector3>();

        for (int x = _itemTilemap.cellBounds.xMin; x < _itemTilemap.cellBounds.xMax; x++)
        {
            for (int y = _itemTilemap.cellBounds.yMin; y < _itemTilemap.cellBounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                Vector3 place = _itemTilemap.CellToWorld(localPlace);
                if (_itemTilemap.HasTile(localPlace))
                {
                    GameObject ballGO = GameObject.Instantiate(prefab, place, Quaternion.identity) as GameObject;
                    ballGO.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    ballGO.GetComponent<Renderer>().material = inMat;
                }
                else
                {
                    GameObject ballGO = GameObject.Instantiate(prefab, place, Quaternion.identity) as GameObject;
                    ballGO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    ballGO.GetComponent<Renderer>().material = outMat;
                }
            }
        }
    }
}
