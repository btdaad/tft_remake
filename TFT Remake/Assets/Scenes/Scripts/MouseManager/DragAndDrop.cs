using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    Camera _camera;
    
    void Start()
    {
        _camera = Camera.main;    
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1000f;
        mousePos = _camera.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);        
    }
}
