using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public abstract class MapAction : IMapAction
    {
        [SerializeField] private string name = "";
        [SerializeField] public List<ConsequenceNeed> consequenceNeeds = new List<ConsequenceNeed>();
        [SerializeField] public List<RequiredNeed> requiredNeeds = new List<RequiredNeed>();

        public abstract void Execute(MapElement executer, MapActionFromMapElement nextActionFromMapElement); //From = executer, To = destination/executionED

        public List<Need> GetNotSatisfiedRequiredNeedsBy(MapElement executer)
        {
            List<Need> requiredNeedsForExecuter = new List<Need>();
            foreach (RequiredNeed requiredNeed in requiredNeeds)
            {
                if (!executer.SatisfiesNeed(requiredNeed))
                    requiredNeedsForExecuter.Add(requiredNeed.need);
            }
            return requiredNeedsForExecuter;
        }

        public string GetName()
        {
            return name.IsNullEmptyOrWhiteSpace() ? this.GetType().Name : name;
        }

        public override string ToString()
        {
            //return this.GetType().Name;
            return this.GetName();
        }
    
        public bool SatisfiesNeed(Need need)
        {
            foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
            {
                if (consequenceNeed.Covers(need))
                    return true;
            }
            return false;
        }

        protected bool CanBeExecuted()
        {
            foreach (RequiredNeed requiredNeed in requiredNeeds)
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}