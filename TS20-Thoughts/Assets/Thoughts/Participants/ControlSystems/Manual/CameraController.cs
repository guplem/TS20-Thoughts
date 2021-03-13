using Essentials.Scripts.Extensions;
using UnityEditor.UIElements;
using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(Manual))]
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject cameraRig;
        private Manual manualControlSystem;

        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float fastSpeedMultiplier = 2f;
        [SerializeField] private float translationTime = 5f;

        [SerializeField] private float rotationSpeed = 1f;

        private Vector3 newPosition;

        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Thoughts.ControlSystems.Manual>();
            newPosition = cameraRig.transform.position;
        }

        private void Update()
        {
            //camera.gameObject.transform.LookAt(cameraRig.transform);
            HandleTranslation();
        }
        
        private void HandleTranslation()
        {
            cameraRig.transform.position = Vector3.Lerp(cameraRig.transform.position, newPosition, Time.deltaTime * translationTime);
        }
        
        public void Move(Vector3 desiredTranslation, bool isFastSpeed)
        {
            newPosition += desiredTranslation * (moveSpeed * (isFastSpeed? fastSpeedMultiplier : 1) );
        }
    }
}
