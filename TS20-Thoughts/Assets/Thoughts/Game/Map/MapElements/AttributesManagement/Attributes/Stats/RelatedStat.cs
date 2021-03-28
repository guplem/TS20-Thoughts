using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class RelatedStat
    {
        [SerializeField] public Stat stat;
        [SerializeField] public int satisfactionAmount = 50;
        [Tooltip("When the satisfaction amount is lower than the minimum demanded, a way to satisfy this need is searched")]
        [SerializeField] private int minimumDemandedSatisfactionAmount = 10;
        public bool needsCare => satisfactionAmount < minimumDemandedSatisfactionAmount;
        public void Apply(ConsequenceStat consequenceStat)
        {
            if (stat.Equals(consequenceStat.stat))
            {
                satisfactionAmount += consequenceStat.deltaSatisfactionAmount;
                //Debug.Log($"Related need {need} at {satisfactionAmount}");
            }
        }
        public bool Satisfies(RequiredStat requiredStat)
        {
            if (stat != requiredStat.stat)
                return false;
            
            return satisfactionAmount > requiredStat.requiredAmount;
        }
    }
}
