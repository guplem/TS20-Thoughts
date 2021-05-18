using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

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

    public void Initialize(MapElement owner)
    {
        ownerMapElement = owner;
        foreach (OwnedAttribute attributeStats in ownedAttributes)
            attributeStats.UpdateOwner(ownerMapElement);
    }

    public override string ToString()
    {
        return $"AttributeManager of {ownerMapElement} with attributes '{ownedAttributes}'";
    }

    public void ExecuteSelfTimeElapseActions()
    {
        if (!ownedAttributes.IsNullOrEmpty())
            foreach (OwnedAttribute attribute in ownedAttributes)
            {
                foreach (MapEvent attributeMapEvent in attribute.attribute.mapEvents)
                {
                    if (attributeMapEvent.executeWithTimeElapse && 
                        attributeMapEvent.GetRequirementsNotMet(ownerMapElement, ownerMapElement, ownerMapElement, out List<int> temp).IsNullOrEmpty())
                    {
                        //Debug.Log($"        · Executing mapEvent '{attributeMapEvent}' of '{attribute}' in '{mapElement}'.");
                        attributeMapEvent.Execute(ownerMapElement, ownerMapElement, ownerMapElement);
                    }
                }
            }
    }

    public void UpdateAttribute(Attribute attributeToUpdate, int deltaValue)
    {
        bool found = false;
        foreach (OwnedAttribute managerAttribute in ownedAttributes)
        {
            if (managerAttribute.attribute == attributeToUpdate)
            {
                managerAttribute.UpdateValue(deltaValue);
                //Debug.Log($"         > The new value for the attribute '{managerAttribute}' in '{ownerMapElement}' is = {managerAttribute.value}");
                found = true;
            }
        }
        if (!found)
        {
            ownedAttributes.Add(new OwnedAttribute(attributeToUpdate, deltaValue, ownerMapElement, false));
        }
    }
    public List<OwnedAttribute> GetAttributesThatNeedCare()
    {
        List<OwnedAttribute> attributesThatNeedCare = new List<OwnedAttribute>();
        foreach (OwnedAttribute attribute in ownedAttributes)
        {
            if (attribute.NeedsCare())
                attributesThatNeedCare.Add(attribute);
        }
        return attributesThatNeedCare;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requirement"></param>
    /// <param name="ownedAttributeThatMostCloselyMeetsTheRequirement">NULL if no attribute can even cover a little bit the requirement</param>
    /// <param name="remainingValueToCoverInAttributeManager"></param>
    /// <returns>True if it contains an attribute with a value higher or equal than the one in the requirement/AttributeUpdate</returns>
    public bool Meets(AttributeUpdate requirement, out OwnedAttribute ownedAttributeThatMostCloselyMeetsTheRequirement, out int remainingValueToCoverInAttributeManager)
    {
        remainingValueToCoverInAttributeManager = requirement.value;
        ownedAttributeThatMostCloselyMeetsTheRequirement = null;
        foreach (OwnedAttribute ownedAttribute in ownedAttributes)
            if (requirement.attribute == ownedAttribute.attribute)
            {
                int remainingValueToCoverWithCurrentAttribute = requirement.value - ownedAttribute.value;
                if (remainingValueToCoverWithCurrentAttribute < remainingValueToCoverInAttributeManager)
                {
                    remainingValueToCoverInAttributeManager = remainingValueToCoverWithCurrentAttribute;
                    ownedAttributeThatMostCloselyMeetsTheRequirement = ownedAttribute;
                }
                
                //Debug.Log($">>>>>> Evaluating if {this} meets the requirement {requirement}. Remaining value = {remainingValueToCoverInAttributeManager}");

                if (remainingValueToCoverInAttributeManager <= 0) // No value is missing, so the requirement can be covered
                {
                    if (ownedAttributeThatMostCloselyMeetsTheRequirement == null)
                        Debug.LogWarning("Meet, but a null attribute shouldn't be the best to cover a requirement....");
                    return true;
                } 
                
            }
                
        //if (ownedAttributeThatMostCloselyMeetsTheRequirement == null)
        //    Debug.LogWarning("No attribute in the AttributeManager can cover the requirement....");

        // Debug.LogWarning($"Requirement of '{requirement.attribute}' not met in '{ownerMapElement}'\n");

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
        Debug.Log($"   Attribute '{attribute}' not found in '{ownerMapElement}' owned attributes. Adding the attribute with a value of 0.\n", ownerMapElement);
        OwnedAttribute newAttribute = new OwnedAttribute(attribute, 0, ownerMapElement, false);
        ownedAttributes.Add(newAttribute);
        return newAttribute;
    }
    public ExecutionPlan GetExecutionPlanToTakeCareOf(OwnedAttribute ownedAttributeToTakeCare, MapElement executer)
    {
        MapElement target = ownedAttributeToTakeCare.ownerMapElement;
        
        //Debug.Log($">>> Searching to take care of '{ownedAttribute.attribute}' owned by '{ownerMapElement}' executed by '{caregiver}'\n", ownerMapElement);
        foreach (OwnedAttribute currentOwnedAttribute in ownedAttributes)
        {
            foreach (MapEvent mapEvent in currentOwnedAttribute.attribute.mapEvents)
            {
                // Debug.Log($" >>> In '{ownerMapElement}', checking '{mapEvent}' in attribute '{currentOwnedAttribute.attribute}' to take care of '{ownedAttributeToTakeCare.attribute}'.\nTarget: {target}, Executer: {executer}, EventOwner still unknown.\n");
                if (!mapEvent.executerMustOwnAttribute || (mapEvent.executerMustOwnAttribute && currentOwnedAttribute.ownerMapElement == executer))
                {
                    if ( mapEvent.tryToCoverRequirementsIfNotMet || (!mapEvent.tryToCoverRequirementsIfNotMet && mapEvent.GetRequirementsNotMet(ownerMapElement, executer, target, out List<int> temp).IsNullOrEmpty()) )
                    {
                        // If reached here, the mapEvent can be executed - Now choose if it is the appropriate one

                        MapElement eventOwner = currentOwnedAttribute.ownerMapElement;
                        
                        /*if (target != executer)
                            eventOwner = target != currentOwnedAttribute.ownerMapElement ? executer : target;*/

                        if (mapEvent.ConsequencesCover(ownedAttributeToTakeCare, target, executer, eventOwner))
                        {
                            ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, eventOwner);
                            // Debug.Log($" ● Found Execution Plan: {executionPlan}");
                            return executionPlan;
                        }   

                    }
                }
                else
                {
                    // Debug.Log($"The executer ({executer}) must own the attribute '{currentOwnedAttribute.attribute}' to execute '{mapEvent}' but it does not. MapEvent owned by '{currentOwnedAttribute.ownerMapElement}'.");
                    if (mapEvent.ConsequencesCover(ownedAttributeToTakeCare, target, executer, executer))
                    {
                        ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, executer);
                        // Debug.Log($" ● Found 'forced' Execution Plan: {executionPlan}");
                        return executionPlan;
                    }
                }
                    
            }
        }

        
        return null;
    }
}