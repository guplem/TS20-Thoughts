using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlan
{
    public MapEvent mapEvent { get; private set; }
    public MapElement eventOwner { get; private set; } //{ get => mapEvent.ownerAttribute.ownerAttributeManager.ownerMapElement; }
    public MapElement executer { get; private set; }
    public MapElement target { get; private set; }

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
        return $"'{mapEvent}' (executed by '{executer}' to target '{target}' at '{eventOwner}')";
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
        return AreRequirementsMet() && IsDistanceMet();
    }
    
    public bool IsDistanceMet()
    {
        return mapEvent.IsDistanceMet(eventOwner, executer);
    }
    
    public bool AreRequirementsMet()
    {
        return mapEvent.AreRequirementsMet(eventOwner, executer, target);
    }
    
}


