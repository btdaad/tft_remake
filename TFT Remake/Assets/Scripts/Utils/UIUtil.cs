using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class UIUtil
{
    public static void HideVisualElements(VisualElement[] visualElements)
    {
        Array.ForEach(visualElements, (VisualElement ve) => { ve.visible = false; });
    }
}
