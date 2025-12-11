using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SortElements : MonoBehaviour
{
    [SerializeField]
    UIDocument movingElements;

    VisualElement baseContainer;

    ItemsDynamicDisplay[] itemsDynamicDisplays;
    
    void Start()
    {
        itemsDynamicDisplays = FindObjectsByType<ItemsDynamicDisplay>(FindObjectsSortMode.None);
        baseContainer = movingElements.rootVisualElement.Q<VisualElement>("ItemInfoContainer");
    }

    void Update()
    {
        baseContainer.Sort(CompareOrder);
    }

    static int CompareOrder(VisualElement x, VisualElement y)
    {
        // Compare the scale of the visual elements in the base container, which is
        // determined by the distance of the object it follows in the ItemsDynamicDisplay component
        return x.style.scale.value.value.x.CompareTo(y.style.scale.value.value.x);
    }
}