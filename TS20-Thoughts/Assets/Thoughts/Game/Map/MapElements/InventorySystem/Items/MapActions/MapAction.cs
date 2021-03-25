using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public abstract class MapAction : IMapAction
    {
        [HideInInspector] public List<ConsequenceNeed> consequenceNeeds = new List<ConsequenceNeed>();
        [HideInInspector] public List<RequireddNeed> requiredNeeds = new List<RequireddNeed>();
        //[HideInInspector] public List<Need> demandedNeeds = new List<Need>();
        
        public abstract void Execute(MapElement executer);

        public bool NeedsToExecuteAreCovered(MapElement executer)
        {
            foreach (RequireddNeed need in requiredNeeds)
            {
                throw new NotImplementedException();
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
            /*foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
            {
                if (consequenceNeed.Solves(need))
                    return true;
            }
            return false;*/
            Debug.LogWarning("SatisfiesNeed? not implemented");
            return true;
        }
    
    }
}