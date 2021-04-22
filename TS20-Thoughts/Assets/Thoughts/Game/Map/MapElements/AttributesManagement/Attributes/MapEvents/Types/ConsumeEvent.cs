using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class ConsumeEvent : MapEvent
{
    public ConsumeEvent() { ;}

    public override void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter)
    {
        //owner.attributeManager.AlterQuantity(attributeOwnerOfEvent, -1);
        foreach (ConsequenceStat consequenceStat in consequenceStats)
        {
            switch (consequenceStat.affected)
            {
                case MapEventStat.Affectation.owner:
                    owner.attributeManager.ApplyConsequence(consequenceStat);
                    break;
                case MapEventStat.Affectation.executer:
                    executer.attributeManager.ApplyConsequence(consequenceStat);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
