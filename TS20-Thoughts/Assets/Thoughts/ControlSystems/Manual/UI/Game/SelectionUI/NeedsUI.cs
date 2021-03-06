using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class NeedsUI : SimpleAnimationUIElement
    {

        /// <summary>
        /// Reference to the NeedGroupUI hosting the information of the Needs with priority = 5
        /// </summary>
        [Tooltip("Reference to the NeedGroupUI hosting the information of the Needs with priority = 5")]
        [SerializeField] private NeedGroupUI needGroup5;
        /// <summary>
        /// Reference to the NeedGroupUI hosting the information of the Needs with priority = 4
        /// </summary>
        [Tooltip("Reference to the NeedGroupUI hosting the information of the Needs with priority = 4")]
        [SerializeField] private NeedGroupUI needGroup4;
        /// <summary>
        /// Reference to the NeedGroupUI hosting the information of the Needs with priority = 3
        /// </summary>
        [Tooltip("Reference to the NeedGroupUI hosting the information of the Needs with priority = 3")]
        [SerializeField] private NeedGroupUI needGroup3;
        /// <summary>
        /// Reference to the NeedGroupUI hosting the information of the Needs with priority = 2
        /// </summary>
        [Tooltip("Reference to the NeedGroupUI hosting the information of the Needs with priority = 2")]
        [SerializeField] private NeedGroupUI needGroup2;
        /// <summary>
        /// Reference to the NeedGroupUI hosting the information of the Needs with priority = 1
        /// </summary>
        [Tooltip("Reference to the NeedGroupUI hosting the information of the Needs with priority = 1")]
        [SerializeField] private NeedGroupUI needGroup1;

        /// <summary>
        /// The MapElement to show the information of 
        /// </summary>
        private MapElement selectedMapElement;
        
        /// <summary>
        /// The last map element from which the in
        /// </summary>
        private MapElement lastShownMapElement;

        /// <summary>
        /// Prepares the NeedUI to be shown
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to show the information of</param>
        public void Setup(MapElement selectedMapElement)
        {
            this.selectedMapElement = selectedMapElement;
        }

        /// <summary>
        /// Populates the NeedGroupUIs with all the needed elements
        /// </summary>
        public void SetupNeedGroups()
        {
            if (lastShownMapElement != selectedMapElement)
            {
                lastShownMapElement = selectedMapElement;
                needGroup5.Setup(selectedMapElement, Attribute.NeedPriority.SelfActualization, nameof(Attribute.NeedPriority.SelfActualization));
                needGroup4.Setup(selectedMapElement, Attribute.NeedPriority.Esteem, nameof(Attribute.NeedPriority.Esteem));
                needGroup3.Setup(selectedMapElement, Attribute.NeedPriority.Love, nameof(Attribute.NeedPriority.Love));
                needGroup2.Setup(selectedMapElement, Attribute.NeedPriority.Safety, nameof(Attribute.NeedPriority.Safety));
                needGroup1.Setup(selectedMapElement, Attribute.NeedPriority.Physiological, nameof(Attribute.NeedPriority.Physiological));
            }
        }

        /// <summary>
        /// Plays the Show or the Hide animation depending on the current state of the UI Element. If state is Show, it plays the Hide animation and vice-versa.
        /// </summary>
        /// <param name="initialStateIsShowing">Should the fist call set the UI Element to the state "hide"?</param>
        public new void SwitchState(bool initialStateIsShowing = false)
        {
            base.SwitchState(initialStateIsShowing);
            
            if (state == VisualizationState.Showing)
                SetupNeedGroups();
        }
        
    }
}
