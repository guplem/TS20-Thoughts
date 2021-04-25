using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlan
{
    public MapEvent mapEvent { get; private set; } // The event to execute
    public MapElement eventOwner { get; private set; } // Who must have the event to be executed
    public MapElement executer { get; private set; } // The executer of the event
    public MapElement target { get; private set; } // The target of the event execution

    public Vector3 executionLocation => eventOwner.transform.position;

    public ExecutionPlan(MapEvent mapEvent, MapElement executer, MapElement target, MapElement eventOwner)
    {
        this.mapEvent = mapEvent;
        this.executer = executer;
        this.target = target;
        this.eventOwner = eventOwner;
    }

    public override string ToString()
    {
        return $"'{mapEvent}' (executed by '{executer}' to target '{target}' owned by '{eventOwner}')";
    }
    
    /// <summary>
    /// If possible, executes the 
    /// </summary>
    /// <returns></returns>
    public bool Execute()
    {
        if (CanBeExecuted())
        {
            mapEvent.Execute(executer, target, eventOwner);
            return true;
        }

        Debug.LogWarning("Trying to execute an ExecutionPlan that can not be executed.");
        return false;
    }
    
    private bool CanBeExecuted()
    {
        List<OwnedAttribute> requirementsNotMet = GetRequirementsNotMet();
        return (requirementsNotMet == null || requirementsNotMet.Count <= 0) && IsDistanceMet();
    }
    
    public bool IsDistanceMet()
    {
        return mapEvent.IsDistanceMet(eventOwner, executer);
    }
    
    public List<OwnedAttribute> GetRequirementsNotMet()
    {
        return mapEvent.GetRequirementsNotMet(eventOwner, executer, target);
    }
    
}


