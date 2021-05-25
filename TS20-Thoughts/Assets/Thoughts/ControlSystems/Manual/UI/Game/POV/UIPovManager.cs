using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class UIPovManager : MonoBehaviour
    {
        [SerializeField]
        public UIAttributesManager uiAttributesManager;

        public void ShowUIFor(MapElement mapElement)
        {
            if (mapElement != null)
                this.transform.parent = mapElement.povCameraPosition;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            uiAttributesManager.ShowUIFor(mapElement);
        }
    }
}
