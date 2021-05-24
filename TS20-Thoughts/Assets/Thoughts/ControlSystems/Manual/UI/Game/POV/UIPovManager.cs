using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class UIPovManager : MonoBehaviour
    {
        [SerializeField]
        public UIAttributesManager uiAttributesManager;

        public void ShowUIFor(Mob mob)
        {
            if (mob != null)
                this.transform.parent = mob.povCameraPrentTransform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            uiAttributesManager.ShowUIFor(mob);
        }
    }
}
