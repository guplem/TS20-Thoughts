using System;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using UnityEngine;

namespace Thoughts.Needs
{
    [System.Serializable]
    public class RequiredStat : MapEventStat
    {
        [SerializeField] public int requiredAmount;
        
        public bool IsSatisfiedBy(MapElement satisfyer, MapElement owner)
        {
            bool result = false;
            
            switch (stat.name)
            {
                case "Closeness":
                    result = Vector3.Distance(satisfyer.transform.position,owner.transform.position) <= 100 - requiredAmount;
                    //Debug.Log($"        -> Closeness is satisfied? : {result}");
                    break;
                
                
                default:
                    result = satisfyer.attributeManager.CanSatisfyStat(this);
                    break;
            }
            
            
            Debug.Log($"        - {satisfyer} does " + (result?"":"not ") + $"satisfy stat {stat}. Required amount = {requiredAmount}.");
            return result;

        }
    }
}
