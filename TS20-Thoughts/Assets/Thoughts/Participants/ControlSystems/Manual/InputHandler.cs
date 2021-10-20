using Thoughts.Game.Map.MapElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Thoughts.Participants.ControlSystems.Manual
{
    /// <summary>
    /// Handles the input for a Manual ControlSystem
    /// </summary>
    [RequireComponent(typeof(Manual))]
    public class InputHandler : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Manual ControlSystem using the input handled by this InputHandler
        /// </summary>
        private Manual manualControlSystem;
        
        /// <summary>
        /// Initial setup
        /// </summary>
        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Manual>();
        }

        //[SerializeField] private KeyCode forward, backward, left, right, up, down;

        private void Update()
        {
            HandleSelection();
            HandleCameraInput();
        }
        
        /// <summary>
        /// Handles the input to control the selection
        /// </summary>
        private void HandleSelection()
        {
            if (IsMouseOverUI())
                return;
            
            if( Input.GetMouseButtonDown(0) )
            {
                Ray ray = manualControlSystem.cameraController.camera.ScreenPointToRay( Input.mousePosition );
                RaycastHit hit;
         
                if( Physics.Raycast( ray, out hit, 100 ) )
                {
                    Debug.Log( $"Clicked on top of the object '{hit.transform.gameObject.name}'" );
                    manualControlSystem.selectedMapElement = hit.transform.gameObject.GetComponentInParent<MapElement>();
                }
                else 
                    manualControlSystem.selectedMapElement = null;
            }
        }
        
        /// <summary>
        /// Checks if the mouse is over a UI element
        /// </summary>
        /// <returns>True if the mouse is over a UI element, otherwise, false .</returns>
        private bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// The start rotation of the mouse.
        /// </summary>
        private Vector3 mouseStartRotation;
        
        /// <summary>
        /// The current rotation of the mouse.
        /// </summary>
        private Vector3 mouseCurrentRotation;
        
        /// <summary>
        /// Handles the input to control the camera
        /// </summary>
        private void HandleCameraInput()
        {
            bool isFastSpeed = Input.GetButton("Shift");
            
            // Movement of the camera
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            manualControlSystem.cameraController.Move(direction, isFastSpeed);

            // Rotation holding middle mouse click
            if (Input.GetButtonDown("Rotation Engager"))
            {
                mouseStartRotation = Input.mousePosition;
            }
            else if (Input.GetButton("Rotation Engager"))
            {
                mouseCurrentRotation = Input.mousePosition;
                Vector3 difference = mouseStartRotation - mouseCurrentRotation;
                mouseStartRotation = mouseCurrentRotation;
                manualControlSystem.cameraController.Rotate(new Vector2(difference.x, difference.y) / 5f, false); // isFastSpeed = false, because there is no need for boost when the velocity is controlled by the user's mouse speed
            }
            
            // Rotation using Q/E keys
            else
                manualControlSystem.cameraController.Rotate(new Vector2(-Input.GetAxis("Rotation"), Input.GetAxis("Zoom")*-100), isFastSpeed);
        }
    }
}
