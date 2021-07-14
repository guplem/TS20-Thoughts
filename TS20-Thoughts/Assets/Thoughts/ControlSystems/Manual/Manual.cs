using Thoughts.ControlSystems.UI;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    /// <summary>
    /// A local manual control of a participant.
    /// </summary>
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(InputHandler))]
    public class Manual : ControlSystem
    {
        /// <summary>
        /// The map element selected by the participant using this ControlSystem.
        /// </summary>
        public MapElement selectedMapElement
        {
            get => _selectedMapElement;
            set
            {
                if (selectedMapElement != value)
                {
                    _selectedMapElement = value;
                    gameUIManager.DisplayUIFor(selectedMapElement);
                    Debug.Log($"New Map Element Selected: {selectedMapElement}");
                    //cameraController.SwitchCamera(selectedMapElement == null? CameraController.CameraView.overworld : CameraController.CameraView.pov, selectedMapElement != null ? selectedMapElement.povCameraPosition.transform : null);
                }
                    
            }
        }
        private MapElement _selectedMapElement;
        
        /// <summary>
        /// The Manager of the UI used by this ControlSystem.
        /// </summary>
        [Tooltip("The Manager of the UI used by this ControlSystem")]
        [SerializeField] private GameUIManager gameUIManager;

        /// <summary>
        /// The CameraController used by this ControlSystem.
        /// </summary>
        public CameraController cameraController { get; private set; }
        
        /// <summary>
        /// The InputHandler used by this ControlSystem.
        /// </summary>
        public InputHandler inputHandler { get; private set; }

        /// <summary>
        /// Initial setup of the ControlSystem
        /// </summary>
        private void Awake()
        {
            cameraController = this.GetComponentRequired<CameraController>();
            inputHandler = this.GetComponentRequired<InputHandler>();
        }

        public override void Initialize(Participant participant)
        {
            base.Initialize(participant);
        }

        public override void Disable()
        {
            base.Disable();
        }

    }
}
