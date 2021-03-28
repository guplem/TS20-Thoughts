using System;
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
