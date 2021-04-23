using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlan
{
    public MapEvent mapEvent { get; private set; }
    //public MapElement owner { get; private set; }
    public MapElement executer { get; private set; }
    public MapElement target { get; private set; }

    public ExecutionPlan(MapEvent mapEvent/*, MapElement owner*/, MapElement executer, MapElement target)
    {
        this.mapEvent = mapEvent;
        //this.owner = owner;
        this.executer = executer;
        this.target = target;
    }

    public override string ToString()
    {
        return $"{mapEvent} (by {executer} to {target})";
    }
    public void Execute()
    {
        mapEvent.Execute(executer, target);
    }
}
