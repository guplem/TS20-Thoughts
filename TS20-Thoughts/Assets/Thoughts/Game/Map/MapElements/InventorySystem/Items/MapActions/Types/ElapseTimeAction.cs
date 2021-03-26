using System;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;

public class ElapseTimeAction : MapAction
{
    public override void Execute(MapElement executer)
    {
        if (!base.CanBeExecuted())
            return;
        
        foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
        {
            executer.inventory.Apply(consequenceNeed);
            //Debug.Log($"Executing {consequenceNeed}");
        }
    }
}
