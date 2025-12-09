using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingNameTag : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_NameTagTemplate;

    [SerializeField]
    UIDocument m_BaseContainerDocument;

    [SerializeField]
    float m_ScaleMultiplier;

    [SerializeField]
    float m_DistanceCullingRange;

    VisualElement m_BaseContainer;
    VisualElement m_NpcNameTag;
    
    private Camera _camera;
    private Transform _itemTransform;
    private bool _isItemDisplayed;

    // void Awake()
    // {

    //     m_NpcNameTag = m_NameTagTemplate.Instantiate();

    //     // Set DynamicTransform hint on the moving element to optimize performance.
    //     m_NpcNameTag.usageHints = UsageHints.DynamicTransform;
    //     m_BaseContainer.Add(m_NpcNameTag);
    //     m_NpcNameTag.style.position = new StyleEnum<Position>(Position.Absolute);
    // }

    public void InitItemDisplay(Camera camera)
    {
        SetCamera(camera);
        _itemTransform = null;
        _isItemDisplayed = false;
        m_NpcNameTag = m_NameTagTemplate.Instantiate();
        m_BaseContainer = m_BaseContainerDocument.rootVisualElement.Q<VisualElement>("BaseContainer");
        m_NpcNameTag.usageHints = UsageHints.DynamicTransform;
        m_BaseContainer.Add(m_NpcNameTag);
        m_NpcNameTag.style.position = new StyleEnum<Position>(Position.Absolute);
        // _unitDisplayBackground = GetUIElement<VisualElement>("UnitDisplayBackground");
        // _unitDisplayBackground.visible = false;
    }

    void Update()
    {
        if (_isItemDisplayed)
            SetNameTagPositionAndScale();
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void ShowItemDisplay(Transform itemTransform)
    {
        _itemTransform = itemTransform;
        _isItemDisplayed = true;
        // _unitDisplayBackground.visible = true;
    }

    public void HideItemDisplay()
    {
        _itemTransform = null;
        _isItemDisplayed = false;
        // UIUtil.HideVisualElements(_traitTextures);
        // UIUtil.HideVisualElements(_items);
        // _unitDisplayBackground.visible = false;
    }

    void SetNameTagPositionAndScale()
    {
        var cameraSpaceLocation = GetCameraSpaceLocation(_itemTransform);
        
        // Use style.translate to set the position of the name tag.
        m_NpcNameTag.style.translate = new Translate(cameraSpaceLocation.x, cameraSpaceLocation.y);

        // Get distance of NPC from camera.
        var distance = Vector3.Distance(_itemTransform.position, _camera.transform.position);
        
        // Calculate 1/distance so the name tag get smaller as the distance gets bigger.
        var scale = 1 / distance * m_ScaleMultiplier;

        m_NpcNameTag.style.scale = new Scale(new Vector2(scale, scale));
        
        // Display name tag based on whether it's in front of the camera and within culling range.
        if (cameraSpaceLocation.z < 0 || distance > m_DistanceCullingRange)
        {
            m_NpcNameTag.style.display = DisplayStyle.None;
        }
        else
        {
            m_NpcNameTag.style.display = DisplayStyle.Flex;
        }
    }

    Vector3 GetCameraSpaceLocation(Transform itemTransform)
    {
        // Get the size of the parent visual element of the name tag.
        var containerSize = m_BaseContainer.layout.size;
        var screenPoint = _camera.WorldToViewportPoint(itemTransform.position);
        var output = new Vector3(screenPoint.x * containerSize.x, (1 - screenPoint.y) * containerSize.y, screenPoint.z);
        
        return output;
    }
}
