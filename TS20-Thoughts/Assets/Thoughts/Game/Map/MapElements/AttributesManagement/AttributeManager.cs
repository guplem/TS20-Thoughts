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
        bool found = false;
        foreach (OwnedAttribute managerAttribute in ownedAttributes)
        {
            if (managerAttribute.attribute == attributeToUpdate)
            {
                managerAttribute.value += deltaValue;
                //Debug.Log($"         > The new value for the attribute '{managerAttribute}' in '{ownerMapElement}' is = {managerAttribute.value}");
                found = true;
            }
        }
        if (!found)
        {
            ownedAttributes.Add(new OwnedAttribute(attributeToUpdate, deltaValue, ownerMapElement, 0, false));
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
        Debug.LogWarning($"Requirement of '{requirement.attribute}' not met in '{ownerMapElement}'\n");
        return false;
    }
    public OwnedAttribute GetOwnedAttributeOf(Attribute attribute)
    {
        foreach (OwnedAttribute ownedAttribute in ownedAttributes)
        {
            if (ownedAttribute.attribute == attribute)
                return ownedAttribute;
        }
        //ToDo: adding the attribute (next lines) should be done in another method. Maybe calling a new method calling 'GetOwnedAttributeAndAddItIfNotFound' should  be created to call them both
        Debug.Log($"Attribute '{attribute}' not found in '{ownerMapElement}' owned attributes. Adding the attribute with a value of 0.\n", ownerMapElement);
        OwnedAttribute newAttribute = new OwnedAttribute(attribute, 0, ownerMapElement, 0, false);
        ownedAttributes.Add(newAttribute);
        return newAttribute;
    }
    public ExecutionPlan GetExecutionPlanToTakeCareOf(OwnedAttribute ownedAttribute, MapElement executer)
    {
        MapElement target = ownedAttribute.ownerMapElement;
        
        //Debug.Log($">>> Searching to take care of '{ownedAttribute.attribute}' owned by '{ownerMapElement}' executed by '{caregiver}'\n", ownerMapElement);
        foreach (OwnedAttribute currentOwnedAttribute in ownedAttributes)
        {
            foreach (MapEvent mapEvent in currentOwnedAttribute.attribute.mapEvents)
            {
                Debug.Log($" >>> Checking '{mapEvent}' in attribute '{currentOwnedAttribute.attribute}' owned by '{ownerMapElement}'. Target: {target}, Executer: {executer}, EventOwner still unknown.");
                if (!mapEvent.executerMustOwnAttribute || (mapEvent.executerMustOwnAttribute && currentOwnedAttribute.ownerMapElement == executer))
                {
                    if ( mapEvent.tryToCoverRequirementsIfNotMet || (!mapEvent.tryToCoverRequirementsIfNotMet && mapEvent.GetRequirementsNotMet(ownerMapElement, executer, target).IsNullOrEmpty()) )
                    {
                        // If reached here, the mapEvent can be executed - Now choose if it is the appropriate one

                        MapElement eventOwner = currentOwnedAttribute.ownerMapElement;
                        
                        /*if (target != executer)
                            eventOwner = target != currentOwnedAttribute.ownerMapElement ? executer : target;*/

                        if (mapEvent.CanCover(ownedAttribute.attribute, target, executer, eventOwner))
                        {
                            ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, eventOwner);
                            Debug.Log($" <> Found Execution Plan: {executionPlan}");
                            return executionPlan;
                        }   

                    }
                }
                else
                {
                    Debug.Log($"The executer ({executer}) must own the attribute '{currentOwnedAttribute.attribute}' to execute '{mapEvent}' but it does not. MapEvent owned by '{currentOwnedAttribute.ownerMapElement}'.");
                    if (mapEvent.CanCover(ownedAttribute.attribute, target, executer, executer))
                    {
                        ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, executer);
                        Debug.Log($" <> Found 'forced' Execution Plan: {executionPlan}");
                        return executionPlan;
                    }
                }
                    
            }
        }

        
        return null;
    }
}