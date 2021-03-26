using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public abstract class MapAction : IMapAction
    {
        [SerializeField] private string name = "";
        [SerializeField] public List<ConsequenceNeed> consequenceNeeds = new List<ConsequenceNeed>();
        [SerializeField] public List<RequiredNeed> requiredNeeds = new List<RequiredNeed>();

        public abstract void Execute(MapElement executer);

        public bool NeedsToExecuteAreCovered(MapElement executer)
        {
            foreach (RequiredNeed need in requiredNeeds)
            {
                throw new NotImplementedException();
            }
            return true;
        }

        public string GetName()
        {
            return name.IsNullEmptyOrWhiteSpace() ? this.GetType().Name : name;
        }

        public override string ToString()
        {
            //return this.GetType().Name;
            return this.GetName();
        }
    
        public bool SatisfiesNeed(Need need)
        {
            /*foreach (ConsequenceNeed consequenceNeed in consequenceNeeds)
            {
                if (consequenceNeed.Solves(need))
                    return true;
            }
            return false;*/
            Debug.LogWarning("SatisfiesNeed? not implemented");
            return true;
        }

        protected bool CanBeExecuted()
        {
            foreach (RequiredNeed requiredNeed in requiredNeeds)
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}