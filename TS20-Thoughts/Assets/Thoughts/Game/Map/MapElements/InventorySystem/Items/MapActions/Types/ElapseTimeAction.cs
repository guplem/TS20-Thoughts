using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Needs;
using UnityEngine;

public class ElapseTimeAction : MapAction
{
    public override void Execute(MapElement executer)
    {
        Debug.Log($"   \\-> Executing ElapseTimeAction by {executer}");
        foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
        {
            List<Need> needs = executer.inventory.GetNeeds(consequenceNeed.GetType());
            foreach (Need need in needs)
                need.UpdateSatisfaction(consequenceNeed.deltaSatisfactionAmount);
        }
    }
    
    public override string GetActionName()
    {
        return "Time elapsing";
    }
}
