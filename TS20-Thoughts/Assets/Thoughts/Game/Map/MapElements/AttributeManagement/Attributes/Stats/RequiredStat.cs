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
        
        public bool IsSatisfiedBy(MapElement executer, MapElement statOwner)
        {
            bool result = false;
            
            switch (stat.name)
            {
                case "Closeness":
                    result = Vector3.Distance(executer.transform.position,statOwner.transform.position) <= 100 - requiredAmount;
                    break;
                
                
                default:
                    result = executer.attributeManager.CanSatisfyStat(this);
                    break;
            }
            
            
            Debug.Log($"{executer} does " + (result?"":"not ") + $"satisfy stat {stat}. Required amount = {requiredAmount}.");
            return result;

        }
    }
}
