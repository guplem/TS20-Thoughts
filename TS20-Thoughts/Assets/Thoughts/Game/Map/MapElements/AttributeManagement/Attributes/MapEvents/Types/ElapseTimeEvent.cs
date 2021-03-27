using System;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class ElapseTimeEvent : MapEvent
{
    public override void Execute(MapElement executer, MapElement elementOwnerOfEvent, Attribute attributeOwnerOfEvent, MapEventFromAttributeAtMapElement nextEventFromAttributeAtMapElement)
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
