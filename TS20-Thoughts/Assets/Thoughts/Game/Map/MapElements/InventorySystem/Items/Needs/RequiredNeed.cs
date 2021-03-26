using System;
using UnityEngine;

namespace Thoughts.Needs
{
    [System.Serializable]
    public class RequiredNeed
    {
        [SerializeField] private Need need;
        [SerializeField] private int requiredAmount;
    }
}
