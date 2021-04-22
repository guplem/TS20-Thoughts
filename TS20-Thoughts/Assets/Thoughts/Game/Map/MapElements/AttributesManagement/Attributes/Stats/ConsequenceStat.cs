using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class ConsequenceStat : MapEventStat
    {
        [SerializeField] public int deltaSatisfactionAmount;

        public override string ToString()
        {
            return $"{stat}: ΔSatisfaction={deltaSatisfactionAmount}";
        }
        public bool Covers(Stat stat)
        {
            return stat == this.stat && deltaSatisfactionAmount > 0;
        }
    }
}
