using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MovingNameTag : MonoBehaviour
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
    private bool _isItemDisplayed;

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
        _isItemDisplayed = false;
        _itemInfo = itemInfoTemplate.Instantiate();
        _itemInfoContainer = itemInfoContainerDocument.rootVisualElement.Q<VisualElement>("ItemInfoContainer");
        _itemInfo.usageHints = UsageHints.DynamicTransform;
        _itemInfoContainer.Add(_itemInfo);
        _itemInfo.style.position = new StyleEnum<Position>(Position.Absolute);

        _itemIcon = GetUIElement<VisualElement>("Icon");
        _itemName = GetUIElement<Label>("ItemName");
        InitStats();
        _description = GetUIElement<Label>("Description");
        _descriptionItallic = GetUIElement<Label>("ItallicDescription");
        _descriptionItallic.style.display = DisplayStyle.None; // TODO if someday we want to use the itallic description, it's here

        _itemInfo.visible = false;
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

    public void ShowItemDisplay(Transform itemTransform)
    {
        _itemTransform = itemTransform;
        Item item = _itemTransform.GetComponent<Item>();
        BaseItemSO baseItemSO = item.GetItem();

        _itemName.text = baseItemSO.itemName;
        _itemIcon.style.backgroundImage = baseItemSO.icon;
        _description.text = baseItemSO.destription;

        DisplayStats(baseItemSO.modifiers);

        _isItemDisplayed = true;
        _itemInfo.visible = true;
    }

    public void HideItemDisplay()
    {
        _itemTransform = null;

        UIUtil.HideVisualElements(_stats);
        _itemInfo.visible = false;
        _isItemDisplayed = false;
    }

    void SetNameTagPositionAndScale()
    {
        var cameraSpaceLocation = GetCameraSpaceLocation(_itemTransform);
        
        // Use style.translate to set the position of the name tag.
        _itemInfo.style.translate = new Translate(cameraSpaceLocation.x, cameraSpaceLocation.y);

        // Get distance of NPC from camera.
        var distance = Vector3.Distance(_itemTransform.position, _camera.transform.position);

        _itemInfo.style.scale = new Scale(new Vector2(scale, scale));
        
        // Display name tag based on whether it's in front of the camera and within culling range.
        if (cameraSpaceLocation.z < 0 || distance > distanceCullingRange)
        {
            _itemInfo.style.display = DisplayStyle.None;
        }
        else
        {
            _itemInfo.style.display = DisplayStyle.Flex;
        }
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
