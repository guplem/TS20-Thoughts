using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;
using Object = System.Object;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Thoughts/Stat", order = 2)]
    public class Stat: ScriptableObject, IEquatable<Stat>, IComparer<Stat>
    {

        [SerializeField] public int priority = 50; //Todo: switch to an enumerator with the levels named (?)

        public override string ToString()
        {
            //return $"{this.name}: L={priority}";
            return this.name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="satisfyer">The MapElement that wants to satisfy the stat</param>
        /// <param name="mapEventInAttributeAtMapElementsToCoverRequiredStats">List of all events needed (in backwards order) in order to make the 'satisfyer' increase the stat's satisfaction.</param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public virtual List<MapEventInAttributeAtMapElement> GetEventsToSatisfyThisStat(MapElement satisfyer, List<MapEventInAttributeAtMapElement> mapEventInAttributeAtMapElementsToCoverRequiredStats = null, int iteration = 0)
        {
            // Check if infinite loop
            if (iteration >= 50)
            {
                Debug.LogWarning($"Iteration {iteration} reached to get an action path to take care of the need '{this.name}'.");
                mapEventInAttributeAtMapElementsToCoverRequiredStats.DebugLog( ",","  \\_Events found so far: ");
                return null;
            }
            
            // Get the event to cover the stat and were to perform it (MapElement)
            MapElement mapElementWithEventToCoverRequiredStat = null;
            Attribute attributeWithEventToCoverRequiredStat = null;
            MapEvent eventToCoverRequiredStat = AppManager.gameManager.map.FindEventToCoverStat(this, satisfyer, out mapElementWithEventToCoverRequiredStat, out attributeWithEventToCoverRequiredStat); // TODO: Use the latest event's map element to find the closest next event to the previous one
            
            // An event to cover the Required Stat has been found
            if (eventToCoverRequiredStat != null)
            {
                // Create returned list
                if (mapEventInAttributeAtMapElementsToCoverRequiredStats == null)
                    mapEventInAttributeAtMapElementsToCoverRequiredStats = new List<MapEventInAttributeAtMapElement>();
                
                // Add the found event to the returned list
                mapEventInAttributeAtMapElementsToCoverRequiredStats.Add(new MapEventInAttributeAtMapElement(eventToCoverRequiredStat, attributeWithEventToCoverRequiredStat, mapElementWithEventToCoverRequiredStat));
                
                // Check if the event can be executed, if not, search how to solve the needed stat
                List<Stat> statsToPerformEvent = eventToCoverRequiredStat.GetRequiredStatsNotSatisfiedBy(MapEventStat.Affectation.executer, satisfyer, mapElementWithEventToCoverRequiredStat);
                if (statsToPerformEvent != null && statsToPerformEvent.Count > 0)
                {
                    //foreach (Need needToPerformAction in needsToPerformAction) //ToDo: multiple enqueued lists - More than one stat might need to be covered
                    List<MapEventInAttributeAtMapElement> mapEventsToCoverRequiredStats =  statsToPerformEvent.ElementAt(0).GetEventsToSatisfyThisStat(satisfyer, mapEventInAttributeAtMapElementsToCoverRequiredStats, iteration + 1);
                    return mapEventsToCoverRequiredStats;
                }
            }
            
            return mapEventInAttributeAtMapElementsToCoverRequiredStats;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Stat) obj);
        }
        public bool Equals(Stat other)
        {
            return other != null && other.name.Equals(this.name);
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public static bool operator ==(Stat left, Stat right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Stat left, Stat right)
        {
            return !Equals(left, right);
        }
        public int Compare(Stat x, Stat y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (ReferenceEquals(null, y))
                return 1;
            if (ReferenceEquals(null, x))
                return -1;
            return x.priority.CompareTo(y.priority);
        }
        
    }
    
}