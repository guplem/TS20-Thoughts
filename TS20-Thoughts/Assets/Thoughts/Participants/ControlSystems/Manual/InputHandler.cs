using System;
using Essentials.Scripts.Extensions;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(Manual))]
    public class InputHandler : MonoBehaviour
    {
        private Manual manualControlSystem;
        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Thoughts.ControlSystems.Manual>();
        }

        //[SerializeField] private KeyCode forward, backward, left, right, up, down;

        private void Update()
        {
            handleCameraInput();
        }
        private void handleCameraInput()
        {
            bool isFastSpeed = Input.GetButton("Shift");
            
            float verticalTranslation = 0f;
            Vector3 desiredTranslation = new Vector3(Input.GetAxis("Horizontal"), verticalTranslation, Input.GetAxis("Vertical")).normalized;
            manualControlSystem.cameraController.Move(desiredTranslation, isFastSpeed);

            manualControlSystem.cameraController.Rotate(Input.GetAxis("Rotation"), isFastSpeed);
        }
    }
}
