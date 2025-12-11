using UnityEngine;

public class DisplayItemInfo : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    UIManager _uiManager;
    Camera _camera;
    private bool _isItemDisplayed;
    private bool _isMouseButtonPressed;

    void Start()
    {
        _uiManager = GameManager.Instance.GetUIManager();
        _camera = gameObject.GetComponent<Camera>();
        _isItemDisplayed = false;
        _isMouseButtonPressed = false;
    }

    void Update()
    {
        if (!_isMouseButtonPressed)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                // display item info
                _uiManager.ShowItemDisplayInWorldView(hit.transform);
                _isItemDisplayed = true;
            }
            else if (_isItemDisplayed)
            {
                // hide item info
                _uiManager.HideItemDisplay();
                _isItemDisplayed = false;
            }

            _isMouseButtonPressed = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        }
        else
        {
            if (_isItemDisplayed)
            {
                _uiManager.HideItemDisplay();
                _isItemDisplayed = false;
            }

            _isMouseButtonPressed = !(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1));
        }
    }
}
