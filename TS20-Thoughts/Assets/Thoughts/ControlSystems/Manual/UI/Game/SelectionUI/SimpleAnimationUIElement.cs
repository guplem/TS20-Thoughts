using System;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    [RequireComponent(typeof(SimpleAnimationsManager))]
    public abstract class SimpleAnimationUIElement : MonoBehaviour
    {
        /// <summary>
        /// A reference to the SimpleAnimationsManager used for the animations of thi UI Eement
        /// </summary>
        protected SimpleAnimationsManager simpleAnimationsManager;

        /// <summary>
        /// Prepares this UI Element to perform its basic functionallity
        /// </summary>
        private void Awake()
        {
            simpleAnimationsManager = gameObject.GetComponentRequired<SimpleAnimationsManager>();
        }

        /// <summary>
        /// Plays the animation to show/display this UI Element
        /// </summary>
        public void Show()
        {
            Debug.Log($"Showing {gameObject.name}");
            simpleAnimationsManager.Play("Show", true);
        }

        /// <summary>
        /// Plays the animation to hide this UI Element
        /// </summary>
        public void Hide()
        {
            Debug.Log($"Hiding {gameObject.name}");
            simpleAnimationsManager.Play("Hide", true);
        }


    }
}