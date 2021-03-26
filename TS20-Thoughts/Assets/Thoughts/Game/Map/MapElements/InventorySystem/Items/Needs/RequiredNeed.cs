using System;
using UnityEngine;

namespace Thoughts.Needs
{
    [System.Serializable]
    public class RequiredNeed
    {
        [SerializeField] public Need need;
        [SerializeField] public int requiredAmount;
    }
}
