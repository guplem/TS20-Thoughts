using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;


public class UIAttributesManager : UIPovRow
{
    [SerializeField] private GameObject uiAttributePrefab;
    private List<UIAttribute> uiAttributes = new List<UIAttribute>();
    [SerializeField] private UIMapEventsManager uiMapEventsManager;
    
    public UIAttribute selectedAttribute
    {
        get => _selectedAttribute;
        set
        {
            if (selectedAttribute != value)
            {
                _selectedAttribute = value;
                Debug.Log($"Attribute Selected: {selectedAttribute}");
                uiMapEventsManager.ShowUIFor(currentMapElement, selectedAttribute != null? selectedAttribute.attribute : null);
            }
                    
        }
    }
    private UIAttribute _selectedAttribute;
    
    private MapElement currentMapElement;

    public void ShowUIFor(MapElement mapElement)
    {
        if (mapElement != currentMapElement)
            selectedAttribute = null;
        currentMapElement = mapElement;
        this.gameObject.SetActive(mapElement != null);
        if (mapElement == null)
            return;

        Clear();
        
        for (int attributeIndex = 0; attributeIndex < mapElement.attributeManager.ownedAttributes.Count; attributeIndex++)
        {
            UIAttribute attribute = Instantiate(uiAttributePrefab, GetLocationPosition(attributeIndex),Quaternion.identity , this.transform).GetComponentRequired<UIAttribute>();
            Transform visualizer = mapElement.transform;
            if (mapElement is Mob mobMapElement)
                visualizer = mobMapElement.povCameraPrentTransform;
            attribute.Initialize(mapElement.attributeManager.ownedAttributes[attributeIndex], visualizer);
            uiAttributes.Add(attribute);
            
            //ToDo: remove next 2 lines to disable automatic selection of an attribute
            if (attributeIndex == 0)
                selectedAttribute = attribute;
        }
        
    }

    public override void Clear()
    {
        base.Clear();
        uiAttributes.Clear();
    }
}
