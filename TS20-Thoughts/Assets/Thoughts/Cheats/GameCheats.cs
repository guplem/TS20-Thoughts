// Base code by Piotr Korzuszek
// Source: https://blog.theknightsofunity.com/implement-game-cheats-subsystem-within-unity

using System;
using Thoughts.Game;
using Thoughts.Game.Map;
using Thoughts.Game.Map.MapElements;
using Thoughts.Participants.ControlSystems.Manual;
using UnityEngine;

namespace Thoughts.Cheats
{
    public class GameCheats : MonoBehaviour
    {

        #region CheatDisplayLock

        /// <summary>
        /// Should the cheats be shown/activated the moment play mode is activated in the editor?
        /// </summary>
        public bool showOnEnteringPlayModeOnEditor = true;
        
        /// <summary>
        /// Activate corner area size by screen width percentage
        /// </summary>
        public float ActivateAreaSize = 0.1f;

        /// <summary>
        /// How many clicks the player should do before cheats list will be visible
        /// </summary>
        public int ClicksCount = 5;

        /// <summary>
        /// How many seconds player have to click/touch the screen
        /// </summary>
        public float WaitTime = 2;

        private float[] _clickTimes;

        private int _clickTimesIndex;

        private bool _active = false;

        void Start()
        {
            // create clicks array and reset it with float.MinValue
            _clickTimes = new float[ClicksCount];
            ResetClicks();
            if (Application.isPlaying && Application.isEditor)
                _active = showOnEnteringPlayModeOnEditor;
        }

        private void ResetClicks()
        {
            for (int i = 0; i < ClicksCount; i++)
            {
                _clickTimes[i] = float.MinValue;
            }
        }

        void Update()
        {
            // check for click or touch and register it
            if (CheckClickOrTouch())
            {
                // click will be registered at time since level load
                _clickTimes[_clickTimesIndex] = Time.timeSinceLevelLoad;
                // each next click will be written on next array index or 0 if overflow
                _clickTimesIndex = (_clickTimesIndex + 1) % ClicksCount;
            }

            // check if cheat list should be activated
            if (ShouldActivate())
            {
                _active = true;
                ResetClicks();
            }
        }

        // checks if cheat list should be activated
        private bool ShouldActivate()
        {
            // check if all click/touches were made within WaitTime
            foreach (float clickTime in _clickTimes)
            {
                if (clickTime < Time.timeSinceLevelLoad - WaitTime)
                {
                    // return false if any of click/touch times has been done earlier
                    return false;
                }
            }

            // if we are here, cheat should be activated
            return true;
        }

        // returns true if there's click or touch within the activate area
        private bool CheckClickOrTouch()
        {
            // convert activation area to pixels
            float sizeInPixels = ActivateAreaSize * Screen.width;

            // get the click/touch position
            Vector2? position = ClickOrTouchPoint();

            if (position.HasValue) // position.HasValue returns true if there is a click or touch
            {
                // check if withing the range
                if (position.Value.x >= Screen.width - sizeInPixels && Screen.height - position.Value.y <= sizeInPixels)
                {
                    return true;
                }
            }

            return false;
        }

        // checks for click or touch and returns the screen position in pixels
        private Vector2? ClickOrTouchPoint()
        {
            if (Input.GetMouseButtonDown(0)) // left mouse click
            {
                return Input.mousePosition;
            }
            else if (Input.touchCount > 0) // one or more touch
            {
                // check only the first touch
                Touch touch = Input.touches[0];

                // it should react only when the touch has just began
                if (touch.phase == TouchPhase.Began)
                {
                    return touch.position;
                }
            }

            // null if there's no click or touch
            return null;
        }

        #endregion
        
        private void DisplayCheat(string cheatName, Action clickedCallback)
        {
            if (GUILayout.Button(cheatName))
            {
                clickedCallback();
            }
        }
        
        void OnGUI()
        {
            if (_active)
            {
                DisplayCheat("Close Cheat Menu", () => _active = false);
                DisplayCheat("Test Cheat 1", () => Debug.Log("Test cheat Activated!"));
                DisplayCheat("Select Humanoid", () => SelectHumanoid());
            }
        }
        private void SelectHumanoid()
        {
            HumanoidsGenerator humanoidGenerator = GameObject.FindObjectOfType<HumanoidsGenerator>();
            MapElement humanoid = humanoidGenerator.transform.GetChild(0).GetComponentRequired<MapElement>();
            Manual manualControlSystem = (Manual)GameManager.instance.localManualParticipant.controlSystem;
            manualControlSystem.selectedMapElement = humanoid;
        }

    }
}
