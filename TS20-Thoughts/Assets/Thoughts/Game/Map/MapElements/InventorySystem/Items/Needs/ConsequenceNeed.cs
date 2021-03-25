using System;
using UnityEngine;

namespace Thoughts.Needs
{
    
    
    [System.Serializable]
    public class ConsequenceNeed
    {
        [SerializeField] private TypeSerializable needType;
        [SerializeField] public int deltaSatisfactionAmount;
    
        public ConsequenceNeed(Need need)
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
