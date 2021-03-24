using System.Collections.Generic;
using Thoughts.MapElements;
using Thoughts.Mobs;
using Thoughts.Needs;
using UnityEngine;

public abstract class MobAction : IMobAction
{
    [HideInInspector]public List<NeedSatisfaction> needsCovered = new List<NeedSatisfaction>();
    
    //public Vector3 performPosition {}; //Todo: implement a way to configure it

    public abstract void Execute(Mob mob);

    public abstract bool CanBeExecuted();
    public abstract string GetActionName();

    public override string ToString()
    {
        //return this.GetType().Name;
        return this.GetActionName();
    }
    public bool CoversNeed(Need need)
    {
        foreach (NeedSatisfaction needSatisfaction in needsCovered)
        {
            if (needSatisfaction.Solves(need))
                return true;
        }
        return false;
    }
}
