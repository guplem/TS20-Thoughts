using System;
using Cinemachine;
using UnityEditor.UIElements;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(Manual))]
    public class CameraController : MonoBehaviour
    {

        [Header("Camera Setup")]
        [SerializeField] private Camera _camera;
        public new Camera camera => _camera;
        [SerializeField] private Transform cameraRig;
        [SerializeField] private CinemachineVirtualCameraBase overworldCamera;
        [SerializeField] private CinemachineVirtualCameraBase povCamera;
        public enum CameraView
        {
            overworld,
            pov
        }
        
        private Manual manualControlSystem;
        
        [Header("Camera Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float fastSpeedMultiplier = 2f;
        [SerializeField] private float movementSmoothing = 5f;

        private Vector3 newPosition;
        private Vector2 newRotation;
        
        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Thoughts.ControlSystems.Manual>();
            newPosition = cameraRig.position;
        }

        private void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }
        
        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
                return newRotation.x;
            if (axisName == "Mouse Y")
                return newRotation.y;
            
            return 0;
        }

        private void Update()
        {
            HandleTransformUpdates();
        }
        
        private void HandleTransformUpdates()
        {
            cameraRig.position = Vector3.Lerp(cameraRig.position, newPosition, Time.deltaTime * movementSmoothing);
        }
        
        public void Move(Vector3 desiredTranslation, bool isFastSpeed)
        {
            newPosition += ((camera.transform.rotation * desiredTranslation).WithY(0f).normalized * (isFastSpeed? moveSpeed*fastSpeedMultiplier : moveSpeed) ) /50;
        }
        
        public void Rotate(Vector2 desiredRotation, bool isFastSpeed)
        {
            newRotation = desiredRotation * (rotationSpeed * (isFastSpeed ? fastSpeedMultiplier : 1));
        }

        public void SwitchCamera(CameraView desiredView, Transform povCameraParent = null)
        {
            overworldCamera.Priority = desiredView == CameraView.overworld ? 1 : 0;

            if (desiredView == CameraView.pov)
            {
                povCamera.Priority = 1;
                povCamera.Follow = povCameraParent;
            }
        }
        
    }
}
