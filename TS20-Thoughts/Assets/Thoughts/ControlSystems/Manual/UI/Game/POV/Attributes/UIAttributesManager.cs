using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIAttributesManager : MonoBehaviour
{
    [SerializeField] private GameObject UIAttributePrefab;
    private List<UIAttributeController> uiAttributeControllers = new List<UIAttributeController>();
    
    public void ShowUIFor(MapElement mapElement)
    {
        this.gameObject.SetActive(mapElement != null);
        if (mapElement == null)
            return;

        Clear();

        for (int attributeIndex = 0; attributeIndex < mapElement.attributeManager.attributes.Count; attributeIndex++)
        {
                        UIAttributeController attributeController = Instantiate(UIAttributePrefab, GetLocationPosition(attributeIndex),Quaternion.identity , this.transform).GetComponentRequired<UIAttributeController>();
                        attributeController.Initialize(mapElement.attributeManager.attributes[attributeIndex]);
                        uiAttributeControllers.Add(attributeController);
        }
        
        foreach (Attribute attribute in mapElement.attributeManager.attributes)
        {

        }
    }
    
    private void Clear()
    {
        this.transform.DestroyAllChildren();
        uiAttributeControllers.Clear();
    }
    
    [Header("Attributes' placement")]
    [Range(0, 100)]
    [SerializeField] private int locationsPreviewAmount = 3;
    [SerializeField] private float attributesSeparation = 1.5f;
    [SerializeField] private float attributesDistance = 1f;
    private void OnDrawGizmosSelected()
    {
        for (int locationIndex = 0; locationIndex < locationsPreviewAmount; locationIndex++)
        {
            Gizmos.color = new Color(0.3f, 0.5f, 0.8f);
            Gizmos.DrawSphere(GetLocationPosition(locationIndex), 0.2f);
        }
    }
    private Vector3 GetLocationPosition(int locationIndex)
    {
        bool leftSide = locationIndex != 0 && locationIndex % 2 == 0;
        float div = ((float) locationIndex) / 2f;
        int locationGridPosition = -(int)div;
        if (!leftSide)
            locationGridPosition = ((int)Math.Ceiling(div));
        return transform.position + new Vector3(locationGridPosition*attributesSeparation,0f,attributesDistance);
    }
    
}
