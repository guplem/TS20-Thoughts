using System;
using Thoughts.Game.GameMap;
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
            HandleSelection();
            HandleCameraInput();
        }
        
        private void HandleSelection()
        {
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
        
        private Vector3 mouseStartRotation, mouseCurrentRotation;
        private void HandleCameraInput()
        {
            bool isFastSpeed = Input.GetButton("Shift");
            
            // Movement
            Vector3 desiredTranslation = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            manualControlSystem.cameraController.Move(desiredTranslation, isFastSpeed);

            // Horizontal rotation
            if (Input.GetButtonDown("Rotation Engager"))
            {
                mouseStartRotation = Input.mousePosition;
            }
            else if (Input.GetButton("Rotation Engager"))
            {
                mouseCurrentRotation = Input.mousePosition;
                Vector3 difference = mouseStartRotation - mouseCurrentRotation;
                mouseStartRotation = mouseCurrentRotation;
                manualControlSystem.cameraController.Rotate(new Vector2(difference.x, difference.y) / 5f, false); // isFastSpeed = false because there is no need for boost when the velocity is controlled by the user's mouse speed
            }
            else
                manualControlSystem.cameraController.Rotate(new Vector2(-Input.GetAxis("Rotation"), Input.GetAxis("Zoom")*-100), isFastSpeed);
        }
    }
}
