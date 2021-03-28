using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "Thoughts/Attribute", order = 1)]
    public class Attribute : ScriptableObject, IEquatable<Stat>, IComparer<Stat>
    {
        [SerializeField] public List<RelatedStat> relatedStats = new List<RelatedStat>();
        [SerializeReference] public List<IMapEvent> events;
        public new string name => base.name;
        
        public MapEvent GetAction(int index)
        {
            if (events.Count > index)
                return (MapEvent) events[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {events.Count}.");
            return null;
        }
        
        public MapEvent GetActionToCoverNeed(Stat stat, MapElement mapElement)
        {
            foreach (IMapEvent iMobAction in events)
            {
                MapEvent mapEvent = (MapEvent) iMobAction;
                //MobAction action = (MobAction) Activator.CreateInstance(actionType.GetType());
                if (mapEvent.SatisfiesNeed(stat))
                {
                    return mapEvent;
                }
            }

            return null;
        }
        public void Apply(ConsequenceStat consequenceStat)
        {
            foreach (RelatedStat relatedNeed in relatedStats)
            {
                relatedNeed.Apply(consequenceStat);
            }
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
        public static bool operator ==(Attribute left, Attribute right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Attribute left, Attribute right)
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

        public void AlterQuantity(int quantity)
        {
            foreach (RelatedStat relatedStat in relatedStats)
            {
                if (relatedStat.stat.name == "Quantity")
                {
                    relatedStat.satisfactionAmount += quantity;
                    Debug.Log($"     The new quantity is {relatedStat.satisfactionAmount}");
                }
            }
        }
    }
    
}

