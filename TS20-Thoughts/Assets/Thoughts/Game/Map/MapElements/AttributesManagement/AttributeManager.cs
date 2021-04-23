using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;
using Object = UnityEngine.Object;

[Serializable]
public class AttributeManager
{
    public MapElement ownerMapElement { get; private set; }

    public List<Attribute> attributes
    {
        get { return _attributes;}
        private set { _attributes = value; }
    }
    [SerializeField] private List<Attribute> _attributes = new List<Attribute>();

    public void Initialize(MapElement newOwner)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            Attribute attributeToInstantiate = attributes[i];
            Attribute instantiatedAttribute = Object.Instantiate(attributeToInstantiate);
            instantiatedAttribute.name = attributeToInstantiate.name;
            attributes[i] = instantiatedAttribute;
        }

        ownerMapElement = newOwner;
        foreach (Attribute attribute in attributes)
            attribute.UpdateOwner(this);
    }
    
    /*public MapEvent GetAvailableActionToCoverAttribute(Attribute attributeToCover, MapElement mapElement, out Attribute attributeWithEventToCoverNeed, out MapEvent mapEventToCoverAttribute)
    {
        foreach (Attribute attribute in attributes)
        {
            MapEvent mapEvent = attribute.GetMapEventToCoverAttribute(attributeToCover, mapElement);
            if (mapEvent != null)
            {
                attributeWithEventToCoverNeed = attribute;
                mapEventToCoverAttribute = mapEvent;
                return mapEvent;
            }
        }

        attributeWithEventToCoverNeed = null;
        mapEventToCoverAttribute = null;
        return null;
    }*/
    
    public void ExecuteTimeElapseActions(MapElement mapElement)
    {
        if (attributes != null)
            foreach (Attribute attribute in attributes)
            {
                foreach (MapEvent attributeMapEvent in attribute.mapEvents)
                {
                    if (attributeMapEvent.executeWithTimeElapse)
                    {
                        //Debug.Log($"        · Executing mapEvent '{attributeMapEvent}' of '{attribute}' in '{mapElement}'.");
                        attributeMapEvent.Execute(mapElement, mapElement);
                    }
                }
            }
    }
    
    /*public void ApplyConsequence(Attribute attribute)
    {
        foreach (MapEvent mapEvent in attribute.mapEvents)
            foreach (AttributeUpdate attributeUpdate in mapEvent.consequenceAttributes)
                attributeUpdate.Apply();
    }*/

    /*public bool CanCoverAttribute(Attribute attributeToCover)
    {
        foreach (Attribute attribute in attributes)
        {
            throw new NotImplementedException();
        }
        return false;
    }*/

    public void UpdateAttribute(Attribute attributeToUpdate, int deltaValue)
    {
        foreach (Attribute managerAttribute in attributes)
        {
            if (managerAttribute == attributeToUpdate)
                managerAttribute.value += deltaValue;
            //Debug.Log($"         > The new value for the attribute '{managerAttribute}' in '{ownerMapElement}' is = {managerAttribute.value}");
        }
    }
    public List<Attribute> GetAttributesThatNeedCare()
    {
        List<Attribute> attributesThatNeedCare = new List<Attribute>();
        foreach (Attribute attribute in attributes)
        {
            if (attribute.takeCare)
                if (attribute.value < attribute.minValue)
                    attributesThatNeedCare.Add(attribute);
        }
        return attributesThatNeedCare;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requirement"></param>
    /// <returns>True if it contains an attribute with a value higher or equal than the one in the requirement/AttributeUpdate</returns>
    public bool Meets(AttributeUpdate requirement)
    {
        foreach (Attribute attribute in attributes)
            if (requirement.attribute == attribute)
                if (requirement.value <= attribute.value)
                    return true;

        return false;
    }
}