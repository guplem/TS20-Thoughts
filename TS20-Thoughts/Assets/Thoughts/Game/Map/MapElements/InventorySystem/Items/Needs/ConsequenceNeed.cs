using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class ConsequenceNeed
    {
        [SerializeField] private Need need;
        [SerializeField] private int deltaSatisfactionAmount;
    }
}
