using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

public class ElapseTimeAction : MapAction
{
    public override void Execute(MapElement executer)
    {
        Debug.Log($"   \\-> Executing ElapseTimeAction by {executer}");
        foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
        {
            List<DemandedNeed> executerDemandedNeeds = executer.inventory.GetDemandedNeedsOf(consequenceNeed.GetType());
            foreach (DemandedNeed need in executerDemandedNeeds)
                need.UpdateSatisfaction(consequenceNeed.deltaSatisfactionAmount);
        }
    }
    
    public override string GetActionName()
    {
        return "Time elapsing";
    }
}
