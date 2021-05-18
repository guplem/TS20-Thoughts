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
    public int executionTimes { get; private set; } // The target of the event execution

    public Vector3 executionLocation
    {
        get
        {
            if (eventOwner == executer)
                return target.transform.position;
            
            if (target != executer)
                return target.transform.position;
            
            return eventOwner.transform.position;
        }
    }

    public ExecutionPlan(MapEvent mapEvent, MapElement executer, MapElement target, MapElement eventOwner, int executionTimes = 1)
    {
        this.mapEvent = mapEvent;
        this.executer = executer;
        this.target = target;
        this.eventOwner = eventOwner;
        this.executionTimes = executionTimes;
    }

    public override string ToString()
    {
        return $"'{mapEvent}' (x{executionTimes} times by '{executer}' to target '{target}' at/owned by '{eventOwner}')";
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
            executionTimes--;
            if (executionTimes > 0)
                return Execute();
            return true;
        }

        Debug.LogWarning("Trying to execute an ExecutionPlan that can not be executed.");
        return false;
    }
    
    private bool CanBeExecuted()
    {
        List<OwnedAttribute> requirementsNotMet = GetRequirementsNotMet(out List<int> temp);
        return (requirementsNotMet.IsNullOrEmpty()) && IsDistanceMet();
    }
    
    public bool IsDistanceMet()
    {
        return mapEvent.IsDistanceMet(target, eventOwner, executer);
    }
    
    public List<OwnedAttribute> GetRequirementsNotMet(out List<int> remainingValueToCoverInRequirementsNotMet)
    {
        return mapEvent.GetRequirementsNotMet(eventOwner, executer, target, out remainingValueToCoverInRequirementsNotMet);
    }

    public int ExecutionsNeededToCover(OwnedAttribute ownedAttributeToCover, int remainingValueToCover)
    {
        int coveredPerExecution = 0;

        //Debug.LogWarning($"Calculating how many times '{this.mapEvent}' must be executed to cover '{ownedAttributeToCover.attribute}'...");

        foreach (AttributeUpdate consequence in mapEvent.consequences)
        {
            //Debug.Log($"CHECKING {consequence.attribute} against {ownedAttributeToCover.attribute}");
            if (consequence.attribute == ownedAttributeToCover.attribute)
            {
                switch (consequence.affected)   
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        if (this.eventOwner == ownedAttributeToCover.ownerMapElement)
                            coveredPerExecution += consequence.value;

                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        if (this.executer == ownedAttributeToCover.ownerMapElement)
                            coveredPerExecution += consequence.value;

                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        if (this.target == ownedAttributeToCover.ownerMapElement)
                            coveredPerExecution += consequence.value;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        int result = -1;
        if (coveredPerExecution > 0)
            result = (int) Math.Ceiling(((double) remainingValueToCover) / ((double) coveredPerExecution));
        
        //Debug.LogWarning($"It needs to be executed {result} times to cover {remainingValueToCover} of remaining value. It covers {coveredPerExecution} per execution.");
        
        if (coveredPerExecution >= remainingValueToCover)
            return 1;
        
        if (coveredPerExecution > 0)
            return result;

        Debug.LogWarning($"{this} can not cover {ownedAttributeToCover}.");
        return -1;
    }
    
    public void SetExecutionsToCover(OwnedAttribute ownedAttributeToCover, int remainingValueToCover)
    {
        executionTimes = ExecutionsNeededToCover(ownedAttributeToCover, remainingValueToCover);
    }
    
}


