using Thoughts.Game.GameMap;
using Thoughts.Mobs;
using UnityEngine;

namespace Thoughts.Needs
{
    public class Hydratation : Need
    {
        public override string GetName()
        {
            return "Drinking water";
        }
        public override bool IsSatisfiedBy(Mob executer)
        {
            Debug.LogWarning("NotImplementedException");
            return true;
        }
    }
}
