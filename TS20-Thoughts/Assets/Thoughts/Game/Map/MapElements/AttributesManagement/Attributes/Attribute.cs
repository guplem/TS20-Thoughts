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

        [SerializeField] public List<MapEvent> mapEvents;

        public override string ToString()
        {
            return name;
        }

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

