using UnityEngine;
using System;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] float dragHeight = 0.0f;
    [SerializeField] float dragThresholdTime = 0.0f;
    GameManager _gameManager;
    BoardManager _boardManager;
    UIManager _uiManager;
    Camera _camera;
    Transform _unitTransform;
    float _unitHeight;
    float _unitDistanceFromCamera;
    float _clickTime;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _boardManager = _gameManager.GetBoardManager();
        _uiManager = _gameManager.GetUIManager();
        _camera = Camera.main;
        _unitTransform = null;
        _unitHeight = 0.0f;
        _unitDistanceFromCamera = 0.0f;
        _clickTime = 0.0f; 
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

                _boardManager.OnDragUnit(_unitTransform);

                _clickTime = Time.time;
            }
            else
                _uiManager.HideUnitDisplay();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - _clickTime < dragThresholdTime)
                _uiManager.ShowUnitDisplay(_unitTransform);
            else // drag
            {
                _boardManager.OnDropUnit(_unitTransform);
            }
            _unitTransform = null;
            _unitHeight = 0.0f;
            _unitDistanceFromCamera = 0.0f;

            _clickTime = 0.0f;
        }
        else if (Time.time - _clickTime >= dragThresholdTime
                 && _unitTransform != null)
        {
            // Debug.Log("Click: " + (Time.time - _clickTime));
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _unitDistanceFromCamera;
            Vector3 unitPosition = _camera.ScreenToWorldPoint(mousePos);
            unitPosition.y = Math.Max(unitPosition.y, _unitHeight);
            unitPosition.y = Math.Min(unitPosition.y, _unitHeight + dragHeight);
            _unitTransform.position = unitPosition;
        }
    }
}
