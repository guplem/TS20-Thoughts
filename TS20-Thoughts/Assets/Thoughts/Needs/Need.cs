
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
        public int satisfaction = 100;
        public int lossSatisfaction = 1;
        [NonSerialized] public const float timeBetweenNeedSatisfactionLoss = 0.1f; // Only applicable for inherit needs (probably only mobs´ hierarchy needs)
        public int threshold = 10;
        public bool needsCare => satisfaction < threshold;

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
            return $"{this.GetType().Name}: P={priority} - V={satisfaction}";
        }

        /// <summary>
        /// Consumes the loss amount from the value.
        /// </summary>
        public void LossSatisfaction()
        {
            satisfaction -= lossSatisfaction;
            
        }
        public virtual List<MobAction> GetActionsToTakeCare(List<MobAction> actions = null, int iteration = 0)
        {
            if (iteration >= 100)
                return null;
            
            if(actions == null)
                actions = new List<MobAction>();
            
            Vector3 positionToPerformAction;
            MobAction actionToCoverNeed = AppManager.currentGame.scenario.FindActionToCoverNeed(this, out positionToPerformAction);

            if (actionToCoverNeed == null)
                return null;
            
            actions.Add(actionToCoverNeed);
            
            if (!actionToCoverNeed.CanBeExecuted())
                return GetActionsToTakeCare(actions, iteration + 1);
            
            return actions;
        }
        
    }
}
