using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Thoughts.Game;
using Thoughts.Game.Map.MapElements;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class SpatialUIManager : ImmediateModeShapeDrawer 
{
        /// <summary>
        /// The currently selected MapElement being displayed in the UI
        /// </summary>
        private MapElement selectedMapElement;
        
        [SerializeField] private float pathElevationAboveTerrain = 0.1f;
        
        /// <summary>
        /// Reference to the section of the spatial UI to display the selected element
        /// </summary>
        [Tooltip("Reference to the section of the spatial UI to display the selected element")]
        [SerializeField] private SelectionSpatialUI selectionSpatialUI;

        /// <summary>
        /// Displays the UI related to the given MapElement and subscribes the UI to changes on that mapElement that should be reflected.
        /// <para>It also unsubscribes the UI to the changes from the previous map element</para>
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        /// <param name="forceUpdate">Ignore if the currently selected MapElement is the same as the new selected MapElement and update the UI as if they were different.</param>
        public void DisplayUIFor(MapElement mapElement, bool forceUpdate = false)
        {
                Debug.Log($"Displaying UI of MapElement: '{mapElement}'");

                if (selectedMapElement != mapElement || forceUpdate)
                {
                        // Unsubscribe to updates
                        if (selectedMapElement != null)
                        {
                                //selectedMapElement.onExecutionPlansUpdated -= behaviorUI.DisplayExecutionPlans;
                                //selectedMapElement.onObjectiveAttributeUpdated -= behaviorUI.DisplayObjectiveAttribute;
                        }
                
                        // Update
                        this.selectedMapElement = mapElement;
                
                        // Activate/Deactivate, get ready and show the current information
                        selectionSpatialUI.Setup(selectedMapElement);

                        // Subscribe to updates
                        if (selectedMapElement != null)
                        {
                                //selectedMapElement.onExecutionPlansUpdated += behaviorUI.DisplayExecutionPlans;
                                //selectedMapElement.onObjectiveAttributeUpdated += behaviorUI.DisplayObjectiveAttribute;
                        }
                }
        }

        private PolylinePath routePolyLinePath;
        private void Awake()
        {
                routePolyLinePath = new PolylinePath(); // a persistent one that can be modified instead of recreated all the time
        }
        void OnDestroy() => routePolyLinePath.Dispose(); // Disposing of mesh data happens here // This ensures any mesh data is cleaned up properly.

        public override void DrawShapes( Camera cam ){

                using( Draw.Command( cam ) ){
                        
                        // More info: https://acegikmo.com/shapes/docs/#immediate-mode
                        /*
                        // Demo drawing disc
                        Draw.Matrix = this.transform.localToWorldMatrix; // will make all following draw calls relative to that 
                        Draw.Color = Color.red; // will make all following Shapes default to red
                        Draw.LineGeometry = LineGeometry.Volumetric3D; // will make lines be drawn using 3D geometry instead of flat quads
                        Draw.ThicknessSpace = ThicknessSpace.Pixels; // will set the thickness space of all shapes to use pixels instead of meters
                        Draw.Thickness = 4; // will make all shapes lines have a width of 4 (pixels, in this case)
                        Draw.Ring(); // Draws a solid disc/filled circle
                        Draw.ResetAllDrawStates(); // will reset everything
                        
                        // set up static parameters. these are used for all following Draw.Line calls
                        
                        Draw.LineGeometry = LineGeometry.Volumetric3D;
                        Draw.ThicknessSpace = ThicknessSpace.Pixels;
                        Draw.Thickness = 4; // 4px wide

                        // set static parameter to draw in the local space of this object
                        Draw.Matrix = transform.localToWorldMatrix;

                        // draw lines
                        Draw.Line( Vector3.zero, Vector3.right,   Color.red   );
                        Draw.Line( Vector3.zero, Vector3.up,      Color.green );
                        Draw.Line( Vector3.zero, Vector3.forward, Color.blue  );
                        */
                        
                        // To draw when a MapElement is selected
                        if (selectedMapElement == null)
                                return;

                        // To draw when a NavMeshAgent is selected
                        if (selectedMapElement.navMeshAgent == null)
                                return;
                        
                        Vector3[] navMeshWaypoints = selectedMapElement.navMeshAgent.path.corners;
                        
                        // To draw then the selected NavMeshAgent has a destination
                        if (navMeshWaypoints.Length <= 1)
                                return;
                        
                        if (routePolyLinePath.Count != selectedMapElement.navMeshAgent.path.corners.Length)
                        {
                                routePolyLinePath.ClearAllPoints();
                                foreach (Vector3 navPoint in navMeshWaypoints)
                                {
                                        Vector2 pathPoint2D = navPoint.ToVector2WithoutY();
                                        Vector3 pathPointProperHeight = pathPoint2D.ToVector3NewY(GameManager.instance.mapManager.GetHeightAt(pathPoint2D) + pathElevationAboveTerrain);
                                        routePolyLinePath.AddPoint(pathPointProperHeight); 
                                }
                                        
                        }
                        else
                                routePolyLinePath.SetPoint(0, selectedMapElement.transform.position);
                        Draw.PolylineJoins = PolylineJoins.Round;
                        Draw.BlendMode = ShapesBlendMode.Opaque;
                        Draw.Polyline( routePolyLinePath, closed:false, thickness:0.125f, Color.white );


                }

        }
}
