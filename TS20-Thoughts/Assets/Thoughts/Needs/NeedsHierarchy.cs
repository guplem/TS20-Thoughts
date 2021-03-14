using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Thoughts.Needs
{
    [Serializable]
    [CreateAssetMenu(fileName = "Needs Hierarchy", menuName = "Thoughts/Needs Hierarchy", order = 1)]
    public class NeedsHierarchy : ScriptableObject
    {
        
        public IReadOnlyList<Need> needs => _needs.Cast<Need>().ToList().AsReadOnly();
        [SerializeReference] private List<INeed> _needs = new List<INeed>();

        public void AddNeed(Need need)
        {
            _needs.Add(need);
            SortNeeds();
        }

        public void SortNeeds()
        {
            _needs.Sort();
        }
        /*public NeedsHierarchy Clone()
        {
            NeedsHierarchy nh = new NeedsHierarchy();
            foreach (Need need in needs)
            {
                nh.AddNeed(need);
            }
            return nh;
        }*/
    }
}
