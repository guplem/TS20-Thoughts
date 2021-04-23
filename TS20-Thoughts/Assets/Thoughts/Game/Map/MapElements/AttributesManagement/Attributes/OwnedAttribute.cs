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
    /// 
    /// </summary>
    /// <param name="attributeOwner">Map element that has the attribute</param>
    /// <param name="caregiver">Map element that wants to take care og the attribute</param>
    /// <returns></returns>
    public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute(MapElement caregiver)
    {
        // Look for all MapEvents that, as consequence of the event, they make the attribute value increase.
        //Be aware that the event consequence can be in 'mode' target, owner or executer 

        List<ExecutionPlan> mapEventsToTakeCare = new List<ExecutionPlan>();
        MapEvent foundMapEvent = null;
        MapElement ownerOfFoundMapEvent = null;
        
        //Trying to take care of an own attribute (in the same MapElement)
        if (ownerMapElement.attributeManager.ownerMapElement == caregiver)
        {
            // 1. Try to solve it with an own event that affect the owner
            foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventOwner, out ownerOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownerOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventExecuter, out ownerOfFoundMapEvent);
            
            // 2. Try to solve it with an external event that affect the executer or the target
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventExecuter, out ownerOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownerOfFoundMapEvent);
        }
        else // Trying to take care of an attribute in another MapElement
        {
            // 3. Try to solve it with an external event that affect the executer or the target
            foundMapEvent = caregiver.GetMapEventToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownerOfFoundMapEvent);
            if (foundMapEvent == null)
                foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(attribute, AttributeUpdate.AttributeUpdateAffected.eventTarget, out ownerOfFoundMapEvent);
        }

        if (foundMapEvent != null)
            mapEventsToTakeCare.Add(new ExecutionPlan(foundMapEvent, caregiver, ownerMapElement.attributeManager.ownerMapElement, ownerOfFoundMapEvent));
        
        //Todo: check if can be performed, if not, try to solve the incenvinience
        return mapEventsToTakeCare;
    }
}
