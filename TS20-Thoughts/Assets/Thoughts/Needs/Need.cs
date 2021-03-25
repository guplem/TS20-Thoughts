using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Needs
{
    [System.Serializable]
    public abstract class Need : INeed, IComparable<Need>
    {
        [SerializeField] public int level = 0; //Todo: switch to an enumerator with the levels named
        public int satisfaction = 100;
        public int lossSatisfaction = 1;
        // [NonSerialized] public const float timeBetweenNeedSatisfactionLoss = 0.1f; // Only applicable for inherit needs (probably only mobs´ hierarchy needs)
        public int threshold = 10;
        public bool needsCare => satisfaction < threshold;

        public int CompareTo(Need other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            return other.level.CompareTo(level);
        }
        public int CompareTo(INeed other)
        {
            return CompareTo((Need) other);
        }

        public virtual string GetName() { return this.GetType().Name; }

        public override string ToString()
        {
            return $"{this.GetType().Name}: L={level} - S={satisfaction}";
        }

        /// <summary>
        /// Consumes the loss amount from the value.
        /// </summary>
        public void LossSatisfaction()
        {
            satisfaction -= lossSatisfaction;
            
        }
        public virtual List<MobAction> GetActionsSatisfy(Mob needyMob, List<MobAction> actions = null, int iteration = 0)
        {
            if (iteration >= 100)
            {
                Debug.LogWarning($"Iteration {iteration}");
                return null;
            }
            
            
            if(actions == null)
                actions = new List<MobAction>();
            
            Vector3 positionToPerformAction;
            MobAction actionToCoverNeed = AppManager.gameManager.map.FindActionToCoverNeed(this, out positionToPerformAction);

            if (actionToCoverNeed == null)
                return null;
            
            actions.Add(actionToCoverNeed);
            
            if (!actionToCoverNeed.NeedsToExecuteAreCovered(needyMob))
                return GetActionsSatisfy(needyMob, actions, iteration + 1);
            
            return actions;
        }

        public abstract bool IsSatisfiedBy(Mob executer);
    }
    
}
