using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemBoardManager
{
    private BoardManager _boardManager;
    private Vector3 _initItemPos;
    private Tilemap _itemTilemap; // Bounds : (-4, -2, 0) to (14, 5, 1)

    // @param side : can be either "Player" or "Opponent"
    public ItemBoardManager(string side, BoardManager boardManager)
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
    }

    public Vector3 GetInitItemPos()
    {
        return _initItemPos;
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

        Vector3 unitPos = new Vector3(itemTransform.position.x, _initItemPos.y, itemTransform.position.z);
        if (!DropOnZone(itemTransform, unitPos, _itemTilemap)) // item is not dropped on the item board
            itemTransform.position = _initItemPos; // restore item position
    }

    public Vector3 GetCellCenterWorldItemBoard(Vector3Int cellPos)
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
        Transform swapItemTransform = _boardManager.GetItemAt(xPos, yPos); // get the item on the drop cell

        _boardManager.SetItemAt(xInitCellPos, yInitCellPos, swapItemTransform); // set grid init cell to swap item
        _boardManager.SetItemAt(xPos, yPos, itemTransform); // set grid drop cell to item 

        if (swapItemTransform != null)
            swapItemTransform.position = new Vector3(_initItemPos.x, swapItemTransform.position.y, _initItemPos.z); // if the swap item exists, move its position
    }
}
