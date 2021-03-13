using Essentials.Scripts.Extensions;
using UnityEditor.UIElements;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(Manual))]
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private new Camera camera;
        [SerializeField] private Transform cameraRig;
        private Manual manualControlSystem;

        [SerializeField] private float moveSpeed = 0.1f;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float fastSpeedMultiplier = 2f;
        [SerializeField] private float movementSmoothing = 5f;

        private Vector3 newPosition;
        private Quaternion newRotation;

        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Thoughts.ControlSystems.Manual>();
            newPosition = cameraRig.position;
            newRotation = cameraRig.rotation;
        }

        private void Update()
        {
            //camera.gameObject.transform.LookAt(cameraRig.transform);
            HandleTransformUpdates();
        }
        
        private void HandleTransformUpdates()
        {
            cameraRig.position = Vector3.Lerp(cameraRig.position, newPosition, Time.deltaTime * movementSmoothing);
            cameraRig.rotation = Quaternion.Lerp(cameraRig.rotation, newRotation, Time.deltaTime * movementSmoothing);
        }
        
        public void Move(Vector3 desiredTranslation, bool isFastSpeed)
        {
            newPosition += cameraRig.rotation * (desiredTranslation * (moveSpeed * (isFastSpeed? fastSpeedMultiplier : 1) ) );
        }
        public void Rotate(float desiredRotation, bool isFastSpeed)
        {
            newRotation *= Quaternion.Euler(Vector3.up * (desiredRotation * (rotationSpeed * (isFastSpeed? fastSpeedMultiplier : 1) ) ) ) ;
        }
    }
}
