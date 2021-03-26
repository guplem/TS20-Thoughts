using System;
using Thoughts.Game.GameMap;

public class ElapseTimeAction : MapAction
{
    public override void Execute(MapElement executer)
    {
        throw new NotImplementedException();
        /*
        Debug.Log($"   \\-> Executing ElapseTimeAction by '{executer}'");
        foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
        {
            Debug.Log($"      |-> Evaluating consequenceNeed '{consequenceNeed}'");
            List<DemandedNeed> executerDemandedNeeds = executer.inventory.GetDemandedNeedsOf(consequenceNeed.GetType());
            Debug.Log($"      |-> Evaluating executerDemandedNeeds '{executerDemandedNeeds}'");
            executerDemandedNeeds.DebugLog(",", "      # executerDemandedNeeds: ");
            foreach (DemandedNeed executerDemandedNeed in executerDemandedNeeds)
                executerDemandedNeed.UpdateSatisfaction(consequenceNeed.deltaSatisfactionAmount);
        }
        */
    }
}
