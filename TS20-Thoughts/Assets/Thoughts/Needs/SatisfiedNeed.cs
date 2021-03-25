using System;
using UnityEngine;

namespace Thoughts.Needs
{
    
    
    [System.Serializable]
    public class SatisfiedNeed
    {
        [SerializeField] private TypeSerializable needType;
        [SerializeField] private int satisfactionAmount;
    
        public SatisfiedNeed(Need need)
        {
            this.needType = new TypeSerializable(need.GetType());
        }
    
        public bool Solves(Need need)
        {
            throw new NotImplementedException();
            //return (needType.Name == need.GetType().Name);
        }
    }
}
