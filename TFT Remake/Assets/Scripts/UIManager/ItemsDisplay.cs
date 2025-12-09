using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class ItemsDisplay
{
    private static ItemsDisplay _instance;
    private static UIDocument _uiDoc;

    public static ItemsDisplay Instance(UIDocument uiDoc)
    { 
        if (_instance == null)
        {
            _instance = new ItemsDisplay();
            _uiDoc = uiDoc;
        }
        return _instance;
    }

    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return _uiDoc.rootVisualElement.Q<T>(name);
    }

    public void InitItemDisplay()
    {
        // _unitDisplayBackground = GetUIElement<VisualElement>("UnitDisplayBackground");
        // _unitDisplayBackground.visible = false;
    }

    public void ShowItemDisplay(Transform itemTransform)
    {
        Item item = itemTransform.GetComponent<Item>();
        // _unitDisplayBackground.visible = true;
    }

    public void HideItemDisplay()
    {
        // UIUtil.HideVisualElements(_traitTextures);
        // UIUtil.HideVisualElements(_items);
        // _unitDisplayBackground.visible = false;
    }
}
