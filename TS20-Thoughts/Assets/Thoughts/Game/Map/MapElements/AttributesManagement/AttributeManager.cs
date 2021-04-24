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

    public List<OwnedAttribute> ownedAttributes
    {
        get { return _ownedAttributes;}
        private set { _ownedAttributes = value; }
    }
    [SerializeField] private List<OwnedAttribute> _ownedAttributes = new List<OwnedAttribute>();
    
    //[SerializeField] private float deleteThis;
    //[SerializeField] private List<DummyClass> dummyClassList = new List<DummyClass>();

    public void Initialize(MapElement owner)
    {
        /*for (int i = 0; i < attributes.Count; i++)
        {
            Attribute attributeToInstantiate = attributes[i];
            Attribute instantiatedAttribute = Object.Instantiate(attributeToInstantiate);
            instantiatedAttribute.name = attributeToInstantiate.name;
            attributes[i] = instantiatedAttribute;
        }*/

        ownerMapElement = owner;
        foreach (OwnedAttribute attributeStats in ownedAttributes)
            attributeStats.UpdateOwner(ownerMapElement);
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
    
    public void ExecuteSelfTimeElapseActions()
    {
        if (ownedAttributes != null)
            foreach (OwnedAttribute attribute in ownedAttributes)
            {
                foreach (MapEvent attributeMapEvent in attribute.attribute.mapEvents)
                {
                    if (attributeMapEvent.executeWithTimeElapse)
                    {
                        //Debug.Log($"        · Executing mapEvent '{attributeMapEvent}' of '{attribute}' in '{mapElement}'.");
                        attributeMapEvent.Execute(ownerMapElement, ownerMapElement, ownerMapElement);
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
        foreach (OwnedAttribute managerAttribute in ownedAttributes)
        {
            if (managerAttribute.attribute == attributeToUpdate)
                managerAttribute.value += deltaValue;
            //Debug.Log($"         > The new value for the attribute '{managerAttribute}' in '{ownerMapElement}' is = {managerAttribute.value}");
        }
    }
    public List<OwnedAttribute> GetAttributesThatNeedCare()
    {
        List<OwnedAttribute> attributesThatNeedCare = new List<OwnedAttribute>();
        foreach (OwnedAttribute attribute in ownedAttributes)
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
        foreach (OwnedAttribute ownedAttribute in ownedAttributes)
            if (requirement.attribute == ownedAttribute.attribute)
                if (requirement.value <= ownedAttribute.value)
                    return true;
        Debug.LogWarning($"Requirement of '{requirement.attribute}' not met in '{ownerMapElement}'");
        return false;
    }
    public OwnedAttribute GetOwnedAttributeOf(Attribute attribute)
    {
        foreach (OwnedAttribute ownedAttribute in ownedAttributes)
        {
            if (ownedAttribute.attribute == attribute)
                return ownedAttribute;
        }
        Debug.LogWarning($"Attribute '{attribute}' not found in '{ownerMapElement}' owned attributes.");
        return null;
    }
}