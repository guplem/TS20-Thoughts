using JetBrains.Annotations;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.SelectionUI
{
    [RequireComponent(typeof(SimpleAnimationsManager))]
    public abstract class SimpleAnimationUIElement : MonoBehaviour
    {
        /// <summary>
        /// A reference to the SimpleAnimationsManager used for the animations of thi UI Eement
        /// </summary>
        protected SimpleAnimationsManager simpleAnimationsManager
        {
            get
            {
                if (_simpleAnimationsManager == null)
                    _simpleAnimationsManager = gameObject.GetComponentRequired<SimpleAnimationsManager>();
                return _simpleAnimationsManager;
            }
        }
        [CanBeNull]
        private SimpleAnimationsManager _simpleAnimationsManager;
        
        /// <summary>
        /// Controls the current state of the AnimationUIElement
        /// </summary>
        protected FlipFlop flipFlop;
        
        /// <summary>
        /// The possible states of the UI Element
        /// </summary>
        public enum VisualizationState
        {
            /// <summary>
            /// The element is being shown/displayed
            /// </summary>
            Showing,
            /// <summary>
            /// The element is being hid/not visible
            /// </summary>
            Hiding
        }

        /// <summary>
        /// The current state of the UI Element.
        /// </summary>
        public VisualizationState state { get; private set; }

        /// <summary>
        /// Plays the Show or the Hide animation depending on the current state of the UI Element. If state is Show, it plays the Hide animation and vice-versa.
        /// </summary>
        /// <param name="initialStateIsShowing">Should the fist call set the UI Element to the state "hide"?</param>
        public void SwitchState(bool initialStateIsShowing = false)
        {
            if (flipFlop == null)
            {
                if (initialStateIsShowing)
                    flipFlop = new FlipFlop(Hide, Show);
                flipFlop = new FlipFlop(Show, Hide);
            }
            flipFlop.Invoke();
        }
        
        /// <summary>
        /// Plays the animation to show/display this UI Element
        /// </summary>
        public void Show()
        {
            //Debug.Log($"Playing simple animation to show an UIElement", this);
            simpleAnimationsManager.Play("Show", false);
            state = VisualizationState.Showing;
        }

        /// <summary>
        /// Plays the animation to hide this UI Element
        /// </summary>
        public void Hide()
        {
            //Debug.Log($"Playing simple animation to hide an UIElement");
            simpleAnimationsManager.Play("Hide", false);
            state = VisualizationState.Hiding;
        }


    }


}