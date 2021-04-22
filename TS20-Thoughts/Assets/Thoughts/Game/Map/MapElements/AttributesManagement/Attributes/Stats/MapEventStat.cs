using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs
{
    [System.Serializable]
    public abstract class MapEventStat
    {
        [SerializeField] public Stat stat;
        [SerializeField] public Affectation affected = Affectation.owner;

        public enum Affectation
        {
            owner,
            executer
        }
    }
}