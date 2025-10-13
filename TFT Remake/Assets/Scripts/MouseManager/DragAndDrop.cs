using UnityEngine;
using System;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] float dragHeight = 0.0f;
    GameManager _gameManager;
    BoardManager _boardManager;
    Camera _camera;
    Transform _unitTransform;
    float _unitHeight;
    float _unitDistanceFromCamera;

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
    }

    void Update()
    {
        if (_unitTransform == null
            && Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
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
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _boardManager.OnDropUnit(_gameManager.isPlayer, _unitTransform);
            ResetInfo();
        }
        else if (_unitTransform != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _unitDistanceFromCamera;
            Vector3 unitPosition = _camera.ScreenToWorldPoint(mousePos);
            unitPosition.y = Math.Max(unitPosition.y, _unitHeight);
            unitPosition.y = Math.Min(unitPosition.y, _unitHeight + dragHeight);
            _unitTransform.position = unitPosition;
        }
    }
}
