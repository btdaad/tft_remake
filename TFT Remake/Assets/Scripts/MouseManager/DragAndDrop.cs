using UnityEngine;
using System;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] LayerMask unitMask;
    [SerializeField] LayerMask itemMask;
    [SerializeField] float dragHeight = 0.0f;
    GameManager _gameManager;
    BoardManager _boardManager;
    Camera _camera;
    Transform _unitTransform;
    float _unitHeight;
    float _unitDistanceFromCamera;
    Transform _itemTransform;
    float _itemHeight;
    float _itemDistanceFromCamera;
    Vector3 _dragOffset;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _boardManager = _gameManager.GetBoardManager();
        _camera = gameObject.GetComponent<Camera>();
        ResetInfo();
    }

    void ResetInfo()
    {
        _unitTransform = null;
        _unitHeight = 0.0f;
        _unitDistanceFromCamera = 0.0f;

        _itemTransform = null;
        _itemHeight = 0.0f;
        _itemDistanceFromCamera = 0.0f;
    }

    void Update()
    {
        if (_unitTransform == null && _itemTransform == null
            && Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, unitMask))
            {
                _unitTransform = hit.transform;
                _unitHeight = _unitTransform.position.y;
                _unitDistanceFromCamera = Vector3.Distance(_camera.transform.position, hit.point);

                bool isUnitPickable = _boardManager.OnDragUnit(_gameManager.isPlayer, _unitTransform);
                if (!isUnitPickable) // if unit is on the opponent board
                {
                    ResetInfo();
                    return;
                }
                else
                    _gameManager.GetUIManager().UpdateSellDisplay(true, (int)_unitTransform.GetComponent<Unit>().stats.cost);
            }

            else if (Physics.Raycast(ray, out hit, 100, itemMask))
            {
                _itemTransform = hit.transform;
                _itemHeight = _itemTransform.position.y;
                _itemDistanceFromCamera = Vector3.Distance(_camera.transform.position, hit.point);
            
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = _itemDistanceFromCamera;
                Vector3 clickPos = _camera.ScreenToWorldPoint(mousePos);
                _dragOffset = _itemTransform.position - clickPos;

                bool isItemPickable = _boardManager.OnDragItem(_gameManager.isPlayer, _itemTransform);
                if (!isItemPickable) // if item is on the opponent board
                {
                    ResetInfo();
                    return;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_gameManager.GetShopManager().IsMouseOverSellingZone())
                _gameManager.GetShopManager().SellUnit(_unitTransform);
            else
            {
                _boardManager.OnDropUnit(_gameManager.isPlayer, _unitTransform);

                if (_itemTransform != null)
                {
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100, unitMask))
                    {
                        Unit unit = hit.transform.GetComponent<Unit>();
                        bool successful = unit.SetItem(_itemTransform.GetComponent<Item>());
                        if (successful)
                            _boardManager.RemoveItem(_gameManager.isPlayer, _itemTransform);
                        else
                            _boardManager.OnDropItem(_gameManager.isPlayer, _itemTransform);
                    }
                    else
                        _boardManager.OnDropItem(_gameManager.isPlayer, _itemTransform);
                }
            }

            _gameManager.GetUIManager().UpdateSellDisplay(false);
            ResetInfo();
        }
        else if (_unitTransform != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _unitDistanceFromCamera;
            Vector3 unitPosition = _camera.ScreenToWorldPoint(mousePos);
            unitPosition.y = Mathf.Clamp(unitPosition.y, _unitHeight, _unitHeight + dragHeight);
            _unitTransform.position = unitPosition;
        }
        else if (_itemTransform != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _itemDistanceFromCamera;
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);

            Vector3 targetPos = worldPos + _dragOffset;
            targetPos.y = Mathf.Clamp(targetPos.y, _itemHeight, _itemHeight + dragHeight);

            _itemTransform.position = targetPos;
        }
    }
}
