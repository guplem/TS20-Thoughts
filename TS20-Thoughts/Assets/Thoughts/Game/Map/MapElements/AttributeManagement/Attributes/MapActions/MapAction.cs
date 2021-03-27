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
        [SerializeField] public List<ConsequenceStat> consequenceStats = new List<ConsequenceStat>();
        [SerializeField] public List<RequiredStat> requiredStats = new List<RequiredStat>();

        public abstract void Execute(MapElement executer, MapActionFromMapElement nextActionFromMapElement); //From = executer, To = destination/executionED

        public List<Stat> GetRequiredNeedsNotSatisfiedBy(MapElement executer)
        {
            List<Stat> requiredNeedsNotSatisfiedByExecuter = new List<Stat>();
            foreach (RequiredStat requiredNeed in requiredStats)
            {
                if (!requiredNeed.IsSatisfiedBy(executer))
                        requiredNeedsNotSatisfiedByExecuter.Add(requiredNeed.stat);
            }
            return requiredNeedsNotSatisfiedByExecuter;
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
    
        public bool SatisfiesNeed(Stat stat)
        {
            foreach (ConsequenceStat consequenceNeed in consequenceStats)
            {
                if (consequenceNeed.Covers(stat))
                    return true;
            }
            return false;
        }

        protected bool CanBeExecuted()
        {
            foreach (RequiredStat requiredNeed in requiredStats)
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}