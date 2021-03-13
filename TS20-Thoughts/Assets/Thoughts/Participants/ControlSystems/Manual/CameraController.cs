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
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float fastSpeedMultiplier = 2f;
        [SerializeField] private float movementSmoothing = 5f;

        private Vector3 newPosition;
        private Quaternion newRotation;
        private Vector3 newZoom;

        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Thoughts.ControlSystems.Manual>();
            newPosition = cameraRig.position;
            newRotation = cameraRig.rotation;
            newZoom = camera.transform.localPosition;
        }

        private void Update()
        {
            HandleTransformUpdates();
        }
        
        private void HandleTransformUpdates()
        {
            cameraRig.position = Vector3.Lerp(cameraRig.position, newPosition, Time.deltaTime * movementSmoothing);
            cameraRig.rotation = Quaternion.Lerp(cameraRig.rotation, newRotation, Time.deltaTime * movementSmoothing);
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, newZoom, Time.deltaTime * movementSmoothing);
        }
        
        public void Move(Vector3 desiredTranslation, bool isFastSpeed)
        {
            newPosition += cameraRig.rotation * (desiredTranslation * (moveSpeed * (isFastSpeed? fastSpeedMultiplier : 1) ) );
        }
        
        public void Rotate(Vector2 desiredRotation, bool isFastSpeed)
        {
            Debug.Log(desiredRotation);
            newRotation *= Quaternion.Euler( (desiredRotation * (rotationSpeed * (isFastSpeed? fastSpeedMultiplier : 1) ) ) ) ;
        }

        public void Zoom(float desiredZoom, bool isFastSpeed)
        {
            newZoom += desiredZoom * zoomSpeed * new Vector3(0, -1, 1);
        }
    }
}
