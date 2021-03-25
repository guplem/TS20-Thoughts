using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public class DemandedNeed
    {
        [SerializeField]
        public TypeSerializable needType;
        [SerializeField] private int demandedAmount = 50;
    
        public DemandedNeed(Need need)
        {
            this.needType = new TypeSerializable(need.GetType());
        }
        public void UpdateSatisfaction(int deltaSatisfactionAmmount)
        {
            throw new System.NotImplementedException();
        }
    }
}
