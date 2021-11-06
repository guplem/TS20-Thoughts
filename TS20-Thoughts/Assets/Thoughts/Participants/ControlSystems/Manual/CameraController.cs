using System;
using Cinemachine;
using Thoughts.Game.Map.MapElements;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual
{
    /// <summary>
    /// A controller for a Camera
    /// </summary>
    public class CameraController : MonoBehaviour
    {

        /// <summary>
        /// The Camera to control
        /// </summary>
        [Header("Camera Setup")]
        [Tooltip("The Camera to control")]
        [SerializeField] private Camera _camera;
        public new Camera camera => _camera;
        
        /// <summary>
        /// The rig of the overworld camera
        /// </summary>
        [Tooltip("The rig of the overworld camera")]
        [SerializeField] private Transform cameraRig;
        
        /// <summary>
        /// Height adjustment layers to use to check for the height that the camera should have
        /// </summary>
        [Tooltip("Height adjustment layers to use to check for the height that the camera should have. Usually just terrain and maybe a few more")]
        [SerializeField] private LayerMask heightAdjustmentLayers;
        
        /// <summary>
        /// Reference to the CinemachineVirtualCamera linked to the overworld view
        /// </summary>
        [Tooltip("Reference to the CinemachineVirtualCamera linked to the overworld view")]
        [SerializeField] private CinemachineVirtualCameraBase overworldCamera;
        
        /// <summary>
        /// Reference to the CinemachineVirtualCamera linked to the POV views
        /// </summary>
        [Tooltip("Reference to the CinemachineVirtualCamera linked to the POV views")]
        [SerializeField] private CinemachineVirtualCameraBase povCamera;
        
        /// <summary>
        /// The possible views/types of cameras available
        /// </summary>
        public enum CameraView
        {
            overworld,
            pov
        }

        /// <summary>
        /// The speed of the camera moving around
        /// </summary>
        [Header("Camera Movement Settings")]
        [Tooltip("The speed of the camera moving around")]
        [SerializeField] private float moveSpeed = 2f;
        
        /// <summary>
        /// The speed of the camera rotating
        /// </summary>
        [Tooltip("The speed of the camera rotating")]
        [SerializeField] private float rotationSpeed = 1.5f;
        
        /// <summary>
        /// Multiplier of the speed when the camera is moved in "fast speed" mode
        /// </summary>
        [Tooltip("Multiplier of the speed when the camera is moved in 'fast speed' mode")]
        [SerializeField] private float fastSpeedMultiplier = 2f;
        
        /// <summary>
        /// Multiplier of the speed when the camera is at its highest position. 0 is none, 1 is double.
        /// </summary>
        [Tooltip("Multiplier of the speed when the camera is at its highest position. 0 is none, 1 is double.")]
        [SerializeField] private float heightBoost = 1.2f;
        
        /// <summary>
        /// The smoothness on the movement of the camera
        /// </summary>
        [Tooltip("The smoothness on the movement of the camera")]
        [Range(0,100)]
        [SerializeField] private float movementSmoothing = 95f;

        /// <summary>
        /// The desired position of the Camera
        /// </summary>
        private Vector3 desiredPosition;
        
        /// <summary>
        /// The desired rotation of the camera
        /// </summary>
        private Vector2 desiredRotation;
        
        /// <summary>
        /// The MapElement followed by the camera
        /// </summary>
        private MapElement followedMapElement;

        /// <summary>
        /// Initial setup
        /// </summary>
        private void Awake()
        {
            desiredPosition = cameraRig.position;
        }

        /// <summary>
        /// Startup setup, after awake
        /// </summary>
        private void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }
        
        /// <summary>
        /// Returns the customized input values overriding the default axis for the Cinemachine camera.
        /// </summary>
        /// <param name="axisName">The name of the axis to get the current value of.</param>
        /// <returns>The current value for the given axis to get to the desired rotation of the camera.</returns>
        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
                return desiredRotation.x;
            if (axisName == "Mouse Y")
                return desiredRotation.y;
            
            return 0;
        }
        
        private void Update()
        {
            HandleTransformUpdates();
        }
        
        /// <summary>
        /// Updates the rig position to get closer to the desired position
        /// </summary>
        private void HandleTransformUpdates()
        {

            if (followedMapElement != null)
                desiredPosition = followedMapElement.transform.position;
            
            Vector3 cameraRigNewPosition = Vector3.Lerp( cameraRig.position, desiredPosition, Time.deltaTime * (100.1f-movementSmoothing) );

            float maxRayDistance = 1000;
            RaycastHit hit;

            if (Physics.Raycast(cameraRigNewPosition + Vector3.up * maxRayDistance, Vector3.down, out hit, maxRayDistance, heightAdjustmentLayers))
            {
                if (hit.collider != null)
                {
                    cameraRigNewPosition = hit.point;
                }
            }

            cameraRig.position = cameraRigNewPosition;
        }
        
        /// <summary>
        /// Moves the camera in a given direction
        /// </summary>
        /// <param name="direction">The desired direction of the camera's translation.</param>
        /// <param name="isFastSpeed">If true, the amount of movement (moveSpeed) will be multiplied by fastSpeedMultiplier</param>
        public void Move(Vector3 direction, bool isFastSpeed)
        {
            float cameraHeight = 0f;

            try
            {
                CinemachineFreeLook freeLookCamera = (CinemachineFreeLook) overworldCamera;
                cameraHeight = freeLookCamera.m_YAxis.Value;
            } catch (Exception ) { }
            
            desiredPosition += ((camera.transform.rotation * direction).WithY(0f).normalized * ( (isFastSpeed? moveSpeed*fastSpeedMultiplier : moveSpeed) * (1f+(cameraHeight*heightBoost)) ) ) /50;
        }
        
        /// <summary>
        /// Sets a new rotation for the camera.
        /// </summary>
        /// <param name="direction">The direction of the desired rotation relative to the current </param>
        /// <param name="isFastSpeed">If true, the amount of rotation (rotationSpeed) will be multiplied by fastSpeedMultiplier</param>
        public void Rotate(Vector2 direction, bool isFastSpeed)
        {
            this.desiredRotation = direction * (rotationSpeed * (isFastSpeed ? fastSpeedMultiplier : 1));
        }

        /// <summary>
        /// Switches the CinemachineVirtualCamera used to handle the camera by switching its priorities.
        /// </summary>
        /// <param name="desiredView">The desired view of the camera</param>
        /// <param name="followTransform">The transform that the camera must follow. Usually not needed (only in some occasions such as POV).</param>
        public void SwitchCamera(CameraView desiredView, Transform followTransform = null)
        {
            overworldCamera.Priority = desiredView == CameraView.overworld ? 1 : 0;

            if (desiredView == CameraView.pov)
            {
                povCamera.Priority = 1;
                povCamera.Follow = followTransform;
            }
        }

        /// <summary>
        /// Forces the camera to follow a MapElement or to stop following it.
        /// </summary>
        /// <param name="followedMapElement">The MapElement to follow, or null if no more following is desired.</param>
        public void FollowMapElement(MapElement followedMapElement)
        {
            this.followedMapElement = followedMapElement;
        }
    }
}
