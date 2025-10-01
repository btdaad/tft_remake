using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Canvas _unitDisplay = null;
    private RawImage _unitInfo = null;
    private RawImage _unitArt = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        _unitDisplay = (Canvas)transform.Find("Unit Display").GetComponent<Canvas>();
        if (_unitDisplay == null)
        {
            Debug.LogError("Could not find a Unit Display canvas.");
            return;
        }

        _unitInfo = (RawImage)_unitDisplay.transform.Find("Unit Info").GetComponent<RawImage>();
        if (_unitInfo == null)
            Debug.LogError("Could not find Unit Info raw image.");
        _unitArt = (RawImage)_unitDisplay.transform.Find("Unit Art").GetComponent<RawImage>();
        if (_unitArt == null)
            Debug.LogError("Could not find Unit Art raw image.");

        _unitDisplay.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowUnitDisplay(Transform unit)
    {
        _unitDisplay.gameObject.SetActive(true);
        _unitArt.material = unit.GetComponent<Renderer>().material;
    }

    public void HideUnitDisplay()
    {
        _unitDisplay.gameObject.SetActive(false);
    }
}
