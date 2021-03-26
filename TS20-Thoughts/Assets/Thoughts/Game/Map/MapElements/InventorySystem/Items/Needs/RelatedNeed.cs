using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class RelatedNeed
    {
        [SerializeField] private Need needType;
        [SerializeField] private int initialSatisfactionAmount = 50;
        [Tooltip("When the satisfaction amount is lower than the minimum demanded, a way to satisfy this need is searched")]
        [SerializeField] private int minimumDemandedSatisfactionAmount;
    }
}
