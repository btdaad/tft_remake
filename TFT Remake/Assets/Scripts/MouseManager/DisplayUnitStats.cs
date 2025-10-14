using UnityEngine;

public class DisplayUnitStats : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    GameManager _gameManager;
    BoardManager _boardManager;
    UIManager _uiManager;
    Camera _camera;
    Transform _unitTransform;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _boardManager = _gameManager.GetBoardManager();
        _uiManager = _gameManager.GetUIManager();
        _camera = gameObject.GetComponent<Camera>();
        _unitTransform = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                _unitTransform = hit.transform;
                _uiManager.ShowUnitDisplay(_unitTransform);
            }
            else
            {
                _uiManager.HideUnitDisplay();
                _unitTransform = null;
            }
        }

        if (_unitTransform != null && _unitTransform.gameObject.activeSelf)
            _uiManager.ShowUnitDisplay(_unitTransform);
        else
        {
            _uiManager.HideUnitDisplay(); // hide stats display if unit has died
            _unitTransform = null;
        }
    }
}
