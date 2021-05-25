using Thoughts.ControlSystems.UI;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(InputHandler))]
    public class Manual : ControlSystem
    {
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
                    cameraController.SwitchCamera(selectedMapElement == null? CameraController.CameraView.overworld : CameraController.CameraView.pov, selectedMapElement != null ? selectedMapElement.povCameraPosition.transform : null);
                }
                    
            }
        }
        private MapElement _selectedMapElement;
        [SerializeField] private GameUIManager gameUIManager;

        public CameraController cameraController { get; private set; }
        public InputHandler inputHandler { get; private set; }

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
