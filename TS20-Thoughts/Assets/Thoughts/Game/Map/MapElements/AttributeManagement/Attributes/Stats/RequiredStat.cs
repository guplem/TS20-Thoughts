using System;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Needs
{
    [System.Serializable]
    public class RequiredStat
    {
        [SerializeField] public Stat stat;
        [SerializeField] public int requiredAmount;
        
        public bool IsSatisfiedBy(MapElement executer)
        {
            return executer.attributeManager.CanSatisfyStat(this);
        }
    }
}
