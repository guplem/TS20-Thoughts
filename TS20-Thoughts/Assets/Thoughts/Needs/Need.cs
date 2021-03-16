
using System;
using System.Collections.Generic;
using Thoughts.MapElements;
using UnityEngine;

namespace Thoughts.Needs
{
    //[CreateAssetMenu(fileName = "Need", menuName = "Thoughts/Need", order = 1)]
    public abstract class Need : INeed, IComparable<Need> // : ScriptableObject
    {
        public int priority = 0;
        public int value = 100;
        public int lossAmount = 1;
        [NonSerialized] public const float timeBetweenLoss = 0.1f;
        public int threshold = 10;
        public bool needsCare => value < threshold;

        public int CompareTo(Need other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            return other.priority.CompareTo(priority);
        }
        public int CompareTo(INeed other)
        {
            return CompareTo((Need) other);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}: P={priority} - V={value}";
        }

        /// <summary>
        /// Consumes the loss amount from the value.
        /// </summary>
        public void Consume()
        {
            value -= lossAmount;
            
        }
        public virtual List<MobAction> GetActionsToTakeCare()
        {
            List<MobAction> actions = new List<MobAction>();
            
            // Where to go
            MapElement elementToCoverNeed = AppManager.currentGame.scenario.FindElementToCoverNeed(this);
            actions.Add(new ConsumeAction(elementToCoverNeed.gameObject.name));
            
            return actions;
        }
        
    }
}
