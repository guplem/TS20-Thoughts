using System;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;

public class ElapseTimeAction : MapAction
{
    public override void Execute(MapElement executer, MapActionFromMapElement nextActionFromMapElement)
    {
        if (!base.CanBeExecuted())
            return;
        
        foreach (ConsequenceStat consequenceNeed in consequenceStats)
        {
            executer.attributeManager.Apply(consequenceNeed);
            //Debug.Log($"Executing {consequenceNeed}");
        }
    }
}
