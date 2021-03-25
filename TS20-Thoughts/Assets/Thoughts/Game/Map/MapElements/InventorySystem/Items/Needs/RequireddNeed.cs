using System;
using UnityEngine;

namespace Thoughts.Needs
{
    
    
    [System.Serializable]
    public class RequireddNeed
    {
        [SerializeField] private TypeSerializable needType;
        [SerializeField] private int requiredAmount;
    
        public RequireddNeed(Need need)
        {
            this.needType = new TypeSerializable(need.GetType());
        }
    
        /*public bool Solves(Need need)
        {
            return (needType.Name == need.GetType().Name);
        }*/
    }
}
