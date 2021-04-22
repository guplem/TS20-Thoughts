using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public abstract class MapEvent : IMapEvent
    {
        [SerializeField] private string _name = "";
        [SerializeField] public List<ConsequenceStat> consequenceStats = new List<ConsequenceStat>();
        [SerializeField] public List<RequiredStat> requiredStats = new List<RequiredStat>();
        public string name
        {
            get
            {
                if (_name.IsNullOrEmpty())
                    return this.GetType().Name;
                return _name;
            }
        }
        
        
        
        public List<Stat> GetRequiredStatsNotSatisfiedBy(MapEventStat.Affectation satisfyer, MapElement executer, MapElement owner)
        {
            //Debug.Log($"    Checking if '{executer}' satisfies the required stats for '{this}' in '{owner}'");
            
            List<Stat> requiredNeedsNotSatisfied = new List<Stat>();
            
            /*if (requiredStats == null || requiredStats.Count <= 0)
                Debug.Log($"        - No required stats exist for '{this}' in '{owner}'.");
            else*/
                foreach (RequiredStat requiredNeed in requiredStats)
                {
                    switch (satisfyer)
                    {
                        case MapEventStat.Affectation.owner:
                            if (requiredNeed.affected == MapEventStat.Affectation.owner)
                                if (!requiredNeed.IsSatisfiedBy(owner, owner))   
                                    requiredNeedsNotSatisfied.Add(requiredNeed.stat);
                            break;
                        case MapEventStat.Affectation.executer:
                            if (requiredNeed.affected == MapEventStat.Affectation.executer)
                                if (!requiredNeed.IsSatisfiedBy(executer, owner))   
                                    requiredNeedsNotSatisfied.Add(requiredNeed.stat);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(satisfyer), satisfyer, null);
                    }

                }
            
            return requiredNeedsNotSatisfied;
        }
        
        public bool CanBeExecuted(MapElement executer, MapElement owner)
        {
            List<Stat> notSatisfiedByExecuter = GetRequiredStatsNotSatisfiedBy(MapEventStat.Affectation.executer, executer, owner);
            List<Stat> notSatisfiedByOwner = GetRequiredStatsNotSatisfiedBy(MapEventStat.Affectation.owner, executer, owner);

            bool result = true;
            
            if (notSatisfiedByExecuter != null && notSatisfiedByExecuter.Count > 0)
                result = false;
            
            if (notSatisfiedByOwner != null && notSatisfiedByOwner.Count > 0)
                result = false;

            return result;
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
        
        //public abstract void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter);
        
        public virtual void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter)
        {
            if (!CanBeExecuted(executer, owner))
            {
                Debug.LogWarning($"Trying to execute event '{this}' but it can not be executed.");
                return;
            }
            
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
}