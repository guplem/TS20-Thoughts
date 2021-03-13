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
            CameraInput();
        }
        private void CameraInput()
        {
            float verticalTranslation = 0f;
            Vector3 desiredTranslation = new Vector3(Input.GetAxis("Horizontal"), verticalTranslation, Input.GetAxis("Vertical")).normalized;
            manualControlSystem.cameraController.Move(desiredTranslation, Input.GetButton("Shift"));
        }
    }
}
