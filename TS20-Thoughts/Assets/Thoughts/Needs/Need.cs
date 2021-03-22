
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
        [NonSerialized] public const float timeBetweenNeedSatisfactionLoss = 0.1f; // Only applicable for inherit needs (probably only mobs´ hierarchy needs)
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
        public void LossSatisfaction()
        {
            value -= lossAmount;
            
        }
        public virtual Queue<MobAction> GetActionsToTakeCare()
        {
            Queue<MobAction> actions = new Queue<MobAction>();

            Item itemToCoverNeed;
            MapElement elementToCoverNeed = AppManager.currentGame.scenario.FindElementToCoverNeed(this, out itemToCoverNeed);

            // 1. Where to go
            actions.Enqueue(new MoveAction(elementToCoverNeed.gameObject.transform.position));
            
            // 2. What to consume
            actions.Enqueue(new ConsumeAction(elementToCoverNeed));
            
            return actions;
        }
        
    }
}
