using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.GameMap
{
    [SerializeField]
    public abstract class MobAction : IMobAction
    {
        [HideInInspector] public List<SatisfiedNeed> satisfiedNeeds = new List<SatisfiedNeed>();
        [HideInInspector] public List<Need> demandedNeeds = new List<Need>();
        //[HideInInspector] public List<string> stringNeeds = new List<string>();

        public abstract void Execute(Mob mob);

        public bool NeedsToExecuteAreCovered(Mob executer)
        {
            foreach (Need need in demandedNeeds)
            {
                if (!need.IsSatisfiedBy(executer))
                    return false;
            }
            return true;
        }
    
        public abstract string GetActionName();

        public override string ToString()
        {
            //return this.GetType().Name;
            return this.GetActionName();
        }
    
        public bool SatisfiesNeed(Need need)
        {
            foreach (SatisfiedNeed needSatisfaction in satisfiedNeeds)
            {
                if (needSatisfaction.Solves(need))
                    return true;
            }
            return false;
        }
    
    }
}