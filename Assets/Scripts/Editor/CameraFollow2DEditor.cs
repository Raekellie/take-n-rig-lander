using UnityEngine;
using UnityEditor;

/// <summary>
/// Customizes the CameraFollow2D script, by hiding and showing variables when they are / aren't needed.
/// </summary>
[CustomEditor( typeof( CameraFollow2D ) )]
public class CameraFollow2DEditor : Editor
{

    #region Variables initialization
    // Reference to the camera follow script.
    private CameraFollow2D camScript;
    // If the zooming settings will be shown.
    private bool showZoomSettings;
    #endregion


    public override void OnInspectorGUI()
    {
        // Reference to the CameraFollow2D script.
        camScript = target as CameraFollow2D;

        // Method to follow.
        camScript.methodToFollow = ( CameraFollow2D.FollowingTypes )EditorGUILayout.EnumPopup( "Method to follow" , camScript.methodToFollow );

        // The target GameObject. (to follow)
        camScript.targetToFollow = EditorGUILayout.ObjectField( "Target GameObject" , camScript.targetToFollow , typeof( Transform ) , true ) as Transform;

        // The time (in seconds) that the camera will take to lock onto the target.
        camScript.lockingTime = EditorGUILayout.FloatField( "Seconds to lock on" , camScript.lockingTime );

        // Camera offset from the target GameObject.
        camScript.camOffset = EditorGUILayout.Vector3Field( "Camera offset" , camScript.camOffset );

        // If zooming is allowed.
        camScript.allowZoom = EditorGUILayout.Toggle( "Allow zoom?" , camScript.allowZoom );

        // If zooming is allowed, show settings for it.
        if ( camScript.allowZoom == true )
        {
            // Show a menu that folds these settings.
            showZoomSettings = EditorGUILayout.Foldout( showZoomSettings , "Zoom settings" , true );

            if ( showZoomSettings == true )
            {
                // Zoom settings.
                camScript.zoomRate = EditorGUILayout.FloatField( "Zooming rate" , camScript.zoomRate );
                /* Quick note about the two variables below this comment:
                 * No, these having the label switched compared to their names isn't a error.
                 * It is written like that so when reading it in the editor, there can be some logic.
                 * (smaller value doesn't mean that it is the minimum zoom, for example).
                 * 
                 * I also do know that I could have switched the variables in the Zoom() method call,
                 * but this way I can change stuff in a easier way.
                 */
                camScript.maxZoom = EditorGUILayout.FloatField( "Minimum zoom" , camScript.maxZoom );
                camScript.minZoom = EditorGUILayout.FloatField( "Maximum zoom" , camScript.minZoom );
            }
        }

    }


}
