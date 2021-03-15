using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Needs Hierarchy", menuName = "Thoughts/Item", order = 2)]
    public class Item : ScriptableObject
    {
        public new string name;
        public bool transferible = false;
        public bool consumible = false;
        //public SerializableDictionary<string, int> needsCovered = new SerializableDictionary<string, int>();
        [HideInInspector] [SerializeReference] public List<INeed> coveredNeeds = new List<INeed>();

        /*public Item()
        {
            Type[] implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            foreach (Type type in implementations)
            {
                needsCovered.Add(type.Name, 0);
            }
        }*/
        
        
    }
    
    /*public interface ICoveredNeed { }

    public abstract class ACoveredNeed
    {
        [SerializeField] public Need need;
        [SerializeField] public int amount;
    }

    [Serializable]
    public class CoveredNeed : ACoveredNeed
    {
        public CoveredNeed(Need need, int amount)
        {
            this.need = need;
            this.amount = amount;
        }
    }*/
}

