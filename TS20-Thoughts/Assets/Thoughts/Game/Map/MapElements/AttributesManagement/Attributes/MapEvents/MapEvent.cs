using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public class MapEvent
    {
        [SerializeField] public string name = "";
        [SerializeField] public List<AttributeUpdate> consequences = new List<AttributeUpdate>();
        [SerializeField] public List<AttributeUpdate> requirements = new List<AttributeUpdate>();
        [SerializeField] public bool executeWithTimeElapse = false;
        [SerializeField] public float maxDistance = 5f;
        //public string name => _name.IsNullEmptyOrWhiteSpace() ? this.GetType().Name : _name;
        //public string name => _name.IsNullEmptyOrWhiteSpace() ? this.GetType().Name : _name;

        /*public List<StatWithMapElement> GetRequiredStatsNotSatisfiedBy(MapElement executer, MapElement owner)
        {
            //Debug.Log($"    Checking if '{executer}' satisfies the required stats for '{this}' in '{owner}'");
            
            List<StatWithMapElement> requiredNeedsNotSatisfied = new List<StatWithMapElement>();
            
                foreach (RequiredStat requiredNeed in requiredStats)
                {
                    if (requiredNeed.affected == MapEventStat.Affectation.owner)
                        if (!requiredNeed.IsSatisfiedBy(owner, owner))   
                            requiredNeedsNotSatisfied.Add(new StatWithMapElement(requiredNeed.stat, owner));

                    if (requiredNeed.affected == MapEventStat.Affectation.executer)
                        if (!requiredNeed.IsSatisfiedBy(executer, owner))   
                            requiredNeedsNotSatisfied.Add(new StatWithMapElement(requiredNeed.stat, executer));
                }
            
            return requiredNeedsNotSatisfied;
        }*/
        
        /*public bool CanBeExecuted(MapElement executer, MapElement owner)
        {
            List<StatWithMapElement> requiredStatsNotSatisfied = GetRequiredStatsNotSatisfiedBy(executer, owner);
            bool result = !(requiredStatsNotSatisfied != null && requiredStatsNotSatisfied.Count > 0);
            return result;
        }*/

        //public abstract void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter);
        
        public void Execute(MapElement executer, MapElement owner, MapElement target)
        {
            Debug.Log($"        · MapElement '{executer}' is executing '{name}' of '{owner}' with target '{target}'.");
            throw new NotImplementedException();
            /*if (!CanBeExecuted(executer, owner))
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
            
            }*/
        }
        
        public override string ToString()
        {
            return this.GetType().Name + (name.IsNullEmptyOrWhiteSpace() ? "" : $" ({name})") ;
        }
    }
}