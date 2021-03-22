using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Inherent Needs Hierarchy", menuName = "Thoughts/Inherent Needs Hierarchy", order = 1)]
    public class InherentNeedsHierarchy : ScriptableObject
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
        
    }
}
