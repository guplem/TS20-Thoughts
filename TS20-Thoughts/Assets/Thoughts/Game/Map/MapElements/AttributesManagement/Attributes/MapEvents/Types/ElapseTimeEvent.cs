using System;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class ElapseTimeEvent : MapEvent
{
    public override void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter)
    {
        base.Execute(executer, owner, attributeOwnerOfEvent, nextEnqueuedEventInExecuter);
    }
}
