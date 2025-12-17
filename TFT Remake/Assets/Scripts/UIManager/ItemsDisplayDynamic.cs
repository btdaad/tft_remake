using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ItemsDynamicDisplay : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset itemInfoTemplate;

    [SerializeField]
    UIDocument itemInfoContainerDocument;

    [SerializeField]
    float scale;

    [SerializeField]
    float distanceCullingRange;

    VisualElement _itemInfoContainer;
    VisualElement _itemInfo;
    
    private Camera _camera;
    private Transform _itemTransform;
    private Vector2 _screenPos;
    private bool _isItemDisplayed;
    private bool _screenSpaceMode; // is the item coordinates in 3D for physical items or 2D for UI items in the unit display
    private const float WORLD_OFFSET_LEFT = 30f;
    private const float WORLD_OFFSET_TOP = -20f;
    private const float EQUIPMENT_OFFSET_LEFT = 50f;
    private const float EQUIPMENT_OFFSET_TOP = -10f;
    private int MAX_STATS_DISPLAYED = 4; // only 4 slots created on the UI

    #region ui-toolkit-content
    private VisualElement _itemIcon;
    private Label _itemName;
    private VisualElement[] _stats;
    private VisualElement[] _statIcons;
    private Label[] _statValues;
    private Label _description;
    private Label _descriptionItallic;
    #endregion ui-toolkit-content
    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return itemInfoContainerDocument.rootVisualElement.Q<T>(name);
    }
    private void InitStats()
    {
        _stats = new VisualElement[MAX_STATS_DISPLAYED];
        _statIcons = new VisualElement[MAX_STATS_DISPLAYED];
        _statValues = new Label[MAX_STATS_DISPLAYED];
        for (int i = 0; i < MAX_STATS_DISPLAYED; i++)
        {
            int statIndex = i + 1;
            _stats[i] = GetUIElement<VisualElement>($"Stat{statIndex}");
            _statIcons[i] = GetUIElement<VisualElement>($"Icon{statIndex}");
            _statValues[i] = GetUIElement<Label>($"Stat{statIndex}");
        }
    }

    public void InitItemDisplay(Camera camera)
    {
        SetCamera(camera);
        _itemTransform = null;
        _screenPos = Vector2.zero;
        _isItemDisplayed = false;
        _itemInfo = itemInfoTemplate.Instantiate();
        _itemInfoContainer = itemInfoContainerDocument.rootVisualElement.Q<VisualElement>("ItemInfoContainer");
        _itemInfo.usageHints = UsageHints.DynamicTransform;
        _itemInfoContainer.Add(_itemInfo);
        _itemInfo.style.position = Position.Absolute;
        _itemInfo.style.display = DisplayStyle.None;

        _itemIcon = GetUIElement<VisualElement>("Icon");
        _itemName = GetUIElement<Label>("ItemName");
        InitStats();
        _description = GetUIElement<Label>("Description");
        _descriptionItallic = GetUIElement<Label>("ItallicDescription");
        _descriptionItallic.style.display = DisplayStyle.None; // TODO if someday we want to use the itallic description, it's here

        _screenSpaceMode = false;
    }

    void Update()
    {
        if (_isItemDisplayed && !_screenSpaceMode) // screen space mode is handled with a callback on geometry changed
            SetItemInfoPositionAndScale();
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    private void DisplayStats(List<BaseItemSO.Modifier> modifiers)
    {
        if (MAX_STATS_DISPLAYED < modifiers.Count)
            Debug.Log("Not enough stats displayed for this item");

        int i = 0;
        while (i < MAX_STATS_DISPLAYED)
        {
            if (i < modifiers.Count)
            {
                BaseItemSO.Modifier modifier = modifiers[i];
                _statIcons[i].style.backgroundImage = Resources.Load<Texture2D>(StatUtil.ToTexture(modifier.stat));

                string value = "+" + modifier.value;
                if (!modifier.isFlat)
                    value += "%";
                _statValues[i].text = value;

                _stats[i].visible = true;
            }
            else
                _stats[i].visible = false;
            i++;
        }
    }

    public void ShowItemDisplayInWorldView(Transform itemTransform)
    {
        _itemTransform = itemTransform;
        Item item = _itemTransform.GetComponent<Item>();

        ShowItemDisplay(item.GetItem());
    }

    private bool _pendingPositionUpdate = false;
    public void ShowItemDisplayAtScreenPoint(Vector2 screenPos, BaseItemSO baseItemSO)
    {
        _screenSpaceMode = true;
        _screenPos = RuntimePanelUtils.ScreenToPanel(itemInfoContainerDocument.rootVisualElement.panel, screenPos);

        ShowItemDisplay(baseItemSO);

        _description.RegisterCallback<GeometryChangedEvent>(OnTooltipGeometryChanged);
        _pendingPositionUpdate = true;
    }

    private void OnTooltipGeometryChanged(GeometryChangedEvent _)
    {
        if (!_pendingPositionUpdate) return;
        _description.UnregisterCallback<GeometryChangedEvent>(OnTooltipGeometryChanged);

        float width = GetUIElement<VisualElement>("SeparatorBar").resolvedStyle.width;

        _itemInfo.style.left = _screenPos.x - width + EQUIPMENT_OFFSET_LEFT;
        _itemInfo.style.top = _screenPos.y + EQUIPMENT_OFFSET_TOP;
        _itemInfo.style.display = DisplayStyle.Flex;

        _pendingPositionUpdate = false;
    }

    private void ShowItemDisplay(BaseItemSO baseItemSO)
    {
        _itemName.text = baseItemSO.itemName;
        _itemIcon.style.backgroundImage = baseItemSO.icon;
        _description.text = baseItemSO.description;

        DisplayStats(baseItemSO.modifiers);

        _isItemDisplayed = true;
        _itemInfo.style.display = DisplayStyle.Flex;
    }

    public void HideItemDisplay()
    {
        _itemTransform = null;
        _screenPos = Vector2.zero;
        _screenSpaceMode = false;

        UIUtil.HideVisualElements(_stats);
        _itemInfo.style.display = DisplayStyle.None;
        _isItemDisplayed = false;
    }

    void SetItemInfoPositionAndScale()
    {
        var cameraSpaceLocation = GetCameraSpaceLocation(_itemTransform);

        // Set the position of the info window.
        _itemInfo.style.left = cameraSpaceLocation.x + WORLD_OFFSET_LEFT;
        _itemInfo.style.top = cameraSpaceLocation.y + WORLD_OFFSET_TOP;

        // Get distance of item from camera.
        var distance = Vector3.Distance(_itemTransform.position, _camera.transform.position);

        // Display item info window based on whether it's in front of the camera and within culling range.
        if (cameraSpaceLocation.z < 0 || distance > distanceCullingRange)
            _itemInfo.style.display = DisplayStyle.None;
        else
            _itemInfo.style.display = DisplayStyle.Flex;

        _itemInfo.style.scale = new Scale(new Vector2(scale, scale));
    }

    Vector3 GetCameraSpaceLocation(Transform itemTransform)
    {
        // Get the size of the parent visual element of the name tag.
        var containerSize = _itemInfoContainer.layout.size;
        var screenPoint = _camera.WorldToViewportPoint(itemTransform.position);
        var output = new Vector3(screenPoint.x * containerSize.x, (1 - screenPoint.y) * containerSize.y, screenPoint.z);
        
        return output;
    }
}
