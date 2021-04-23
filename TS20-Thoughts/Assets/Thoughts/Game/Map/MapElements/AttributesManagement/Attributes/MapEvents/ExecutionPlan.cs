using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlan
{
    
    public MapEvent mapEvent { get; private set; }
    public MapElement eventOwner { get => mapEvent.ownerAttribute.ownerAttributeManager.ownerMapElement; }
    public MapElement executer { get; private set; }
    public MapElement target { get; private set; }

    public Vector3 executionLocation => eventOwner.transform.position;

    public ExecutionPlan(MapEvent mapEvent/*, MapElement owner*/, MapElement executer, MapElement target)
    {
        this.mapEvent = mapEvent;
        this.executer = executer;
        this.target = target;
    }

    public override string ToString()
    {
        return $"{mapEvent} (by '{executer}' to '{target}' at '{eventOwner}')";
    }
    
    /// <summary>
    /// If possible, executes the 
    /// </summary>
    /// <returns></returns>
    public bool Execute()
    {
        if (CanBeExecuted())
        {
            mapEvent.Execute(executer, target);
            return true;
        }

        Debug.LogWarning("Trying to execute an ExecutionPlan that can not be executed.");
        return false;
    }
    
    private bool CanBeExecuted()
    {
        return AreRequirementsMet() && IsDistanceMet();
    }
    
    public bool IsDistanceMet()
    {
        Vector3 eventOwnerPosition = executionLocation;
        Vector3 executerPosition = executer.transform.position;
        return Vector3.Distance(eventOwnerPosition, executerPosition) <= mapEvent.maxDistance;
    }
    
    public bool AreRequirementsMet()
    {
        bool result;
        foreach (AttributeUpdate requirement in mapEvent.requirements)
        {
            switch (requirement.affected)
            {
                case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                    result = eventOwner.attributeManager.Meets(requirement);
                    if (!result) return false;
                break;
                case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                    result = executer.attributeManager.Meets(requirement);
                    if (!result) return false;
                break;
                case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                    result = target.attributeManager.Meets(requirement);
                    if (!result) return false;
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return true;
    }
}


