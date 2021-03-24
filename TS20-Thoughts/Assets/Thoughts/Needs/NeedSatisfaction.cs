using System;
using UnityEngine;

namespace Thoughts.Needs
{
    [Serializable]
    public class NeedSatisfaction
    {
        [SerializeField] private TypeSerializable needType;
        [SerializeField] private int satisfactionAmount;
    
        public NeedSatisfaction(Need need)
        {
            this.needType = new TypeSerializable(need.GetType());
        }
    
        public bool Solves(Need need)
        {
            return (needType.Name == need.GetType().Name);
        }
    }
}
