using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "Thoughts/Attribute", order = 1)]
    public class Attribute : ScriptableObject, IEquatable<Attribute>
    {
        [SerializeField] public List<RelatedStat> relatedStats = new List<RelatedStat>();
        [SerializeReference] public List<IMapEvent> mapEvents;
        public new string name => base.name;
        
        public MapEvent GetAction(int index)
        {
            if (mapEvents.Count > index)
                return (MapEvent) mapEvents[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {mapEvents.Count}.");
            return null;
        }
        
        public MapEvent GetActionToCoverNeed(Stat stat, MapElement mapElement)
        {
            foreach (IMapEvent iMobAction in mapEvents)
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
            return Equals((Attribute) obj);
        }
        public bool Equals(Attribute other)
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

