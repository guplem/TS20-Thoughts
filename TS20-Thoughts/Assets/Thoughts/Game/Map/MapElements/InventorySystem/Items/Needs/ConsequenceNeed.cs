using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class ConsequenceNeed
    {
        [SerializeField] public Need need;
        [SerializeField] public int deltaSatisfactionAmount;

        public override string ToString()
        {
            return $"{need}: ΔSatisfaction={deltaSatisfactionAmount}";
        }
    }
}
