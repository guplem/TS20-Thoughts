using System;
using System.Collections.Generic;
using Thoughts.Game;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "Thoughts/Attribute", order = 1)]
    public class Attribute : ScriptableObject, IEquatable<Attribute>
    {
        
    #region Funtionallity

        [SerializeField] public List<MapEvent> mapEvents;


        
        
        
        /*public void AlterQuantity(int quantity)
        {
            foreach (RelatedStat relatedStat in relatedStats)
            {
                if (relatedStat.stat.name == "Quantity")
                {
                    relatedStat.satisfactionAmount += quantity;
                    Debug.Log($"     The new quantity is {relatedStat.satisfactionAmount}");
                }
            }
        }*/

        /*public MapEvent GetAction(int index)
        {
            if (mapEvents.Count > index)
                return (MapEvent) mapEvents[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {mapEvents.Count}.");
            return null;
        }*/
        
        /*public MapEvent GetMapEventToCoverAttribute(Attribute attribute, MapElement mapElement)
        {
            throw new NotImplementedException();
            return null;
        }*/
    
    #endregion
        
    #region Comparasions
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
            if (left is null && right is null)
                return true;
            if ((left is null) && !(right is null))
                return false;
            if (!(left is null) && (right is null))
                return false;
            return left.Equals(right);
        }
        
        public static bool operator !=(Attribute left, Attribute right)
        {
            return !(left == right);
        }
    #endregion
        
    }
    
}

