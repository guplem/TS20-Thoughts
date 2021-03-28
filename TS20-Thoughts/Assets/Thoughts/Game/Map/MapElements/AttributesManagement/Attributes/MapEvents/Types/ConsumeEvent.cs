using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ConsumeEvent : MapEvent
{
    public ConsumeEvent() { ;}

    public override void Execute(MapElement executer, MapElement elementOwnerOfEvent, Attribute attributeOwnerOfEvent, MapEventFromAttributeAtMapElement nextEventFromAttributeAtMapElement)
    {
        elementOwnerOfEvent.attributeManager.AlterQuantity(attributeOwnerOfEvent, -1);
    }
}
