using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;

[Serializable]
public class OwnedAttribute
{
    [SerializeField] public Thoughts.Attribute attribute;
    [SerializeField] public float value;
    [SerializeField] public float minValue;
    [SerializeField] public bool takeCare;
    
    public MapElement ownerMapElement { get; private set; }
    public void UpdateOwner(MapElement newOwner)
    {
        this.ownerMapElement = newOwner;
    }
    
    
    /// <summary>
    /// Look for all MapEvents that, as consequence of the event, they make the attribute value increase for the owner/executer/target (the needed participant).
    /// </summary>
    /// <param name="caregiver">Map element that wants to take care og the attribute</param>
    /// <returns></returns>
    public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute(MapElement caregiver, List<ExecutionPlan> mapEventsToTakeCare = null, int iteration = 0)
    {
        if (iteration >= 50)
        {
            Debug.LogWarning($"Stopping the search of an execution plan for {attribute} after {iteration} iterations.");
            return null;
        }
        
        if (mapEventsToTakeCare == null) 
            mapEventsToTakeCare = new List<ExecutionPlan>();
        MapEvent foundMapEvent = null;
        OwnedAttribute ownedAttributeOfFoundMapEvent = null;
        
        //Trying to take care of an own attribute (in the same MapElement)
        if (ownerMapElement.attributeManager.ownerMapElement == caregiver)
        {
            // 1. Try to solve it with an own event that affect the owner
            foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventOwner, out ownedAttributeOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownedAttributeOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventExecuter, out ownedAttributeOfFoundMapEvent);
            
            // 2. Try to solve it with an external event that affect the executer or the target
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventExecuter, out ownedAttributeOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownedAttributeOfFoundMapEvent);
        }
        else // Trying to take care of an attribute in another MapElement
        {
            // 3. Try to solve it with an external event that affect the executer or the target
            foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownedAttributeOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownedAttributeOfFoundMapEvent);
        }

        if (foundMapEvent != null)
            mapEventsToTakeCare.Add(new ExecutionPlan(foundMapEvent, caregiver, ownerMapElement.attributeManager.ownerMapElement, ownedAttributeOfFoundMapEvent.ownerMapElement));

        if (ownedAttributeOfFoundMapEvent != null)
        {
            ExecutionPlan foundExecutionPlan = mapEventsToTakeCare[mapEventsToTakeCare.Count - 1];
            if (!foundExecutionPlan.AreRequirementsMet())
                return ownedAttributeOfFoundMapEvent.GetExecutionPlanToCoverThisAttribute(caregiver, mapEventsToTakeCare, iteration+1);
        }
        else
        {
            Debug.LogWarning($"Execution plan for covering '{attribute}' in '{ownerMapElement}' not found.");
        }

        return mapEventsToTakeCare;
    }
}
