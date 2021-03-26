using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class RelatedNeed
    {
        [SerializeField] public Need need;
        [SerializeField] public int satisfactionAmount = 50;
        [Tooltip("When the satisfaction amount is lower than the minimum demanded, a way to satisfy this need is searched")]
        [SerializeField] private int minimumDemandedSatisfactionAmount;
        public void Apply(ConsequenceNeed consequenceNeed)
        {
            if (need.Equals(consequenceNeed.need))
            {
                satisfactionAmount += consequenceNeed.deltaSatisfactionAmount;
                //Debug.Log($"Related need {need} at {satisfactionAmount}");
            }
        }
    }
}
