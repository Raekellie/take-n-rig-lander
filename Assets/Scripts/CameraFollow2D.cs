using UnityEngine;

/// <summary>
/// This script will make a camera follow a target. It follows X and Y coordinates, as Z will be static.
/// How to use: 
/// First, attach this script to any GameObject with a Camera component (will be automatically added if there isn't)
/// Then, change the desired variables in the Inspector and everything should then be ready.
/// </summary>
[RequireComponent( typeof( Camera ) )]
public class CameraFollow2D : MonoBehaviour
{

    // The time the camera takes to lock onto the target.
    public float lockingTime = 0.15f;

    // Target GameObject for the camera to follow.
    public Transform targetToFollow;

    // Allow zooming into the target? (by using the mouse scroll wheel)
    public bool allowZoom = false;

    // The rate at the camera will zoom into the target
    // (only if zooming is allowed, defined in the boolean variable above)
    public float zoomRate = 2f;

    // Minimum and maximum zoom.
    public float minZoom = 5f;
    public float maxZoom = 15f;

    // This camera variable.
    private Camera cam;

    // Camera offset
    public Vector3 camOffset;


    // Velocity.
    private Vector3 velocity = Vector3.zero;

    // How the camera will follow the target.
    public enum FollowingTypes
    {
        Smooth,
        Fast,
    };
    // Be able to choose the method to follow on Unity's Inspector.
    public FollowingTypes methodToFollow;

    // If the message noticing that the target no longer exists has already been printed.
    private bool nullObjectMessageShown = false;



    private void Start()
    {
        // Get the camera of the GameObject that this script is attached to and store it.
        cam = GetComponent<Camera>();
    }


    private void FixedUpdate()
    {
        // Check if the target to follow exists.
        if ( targetToFollow != null )
        {
            // Follow the target depending on the method to follow choice.
            if ( methodToFollow == FollowingTypes.Smooth )
            {
                SmoothFollow( targetToFollow );
            }
            else if ( methodToFollow == FollowingTypes.Fast )
            {
                FastFollow( targetToFollow );
            }

            // If the mouse scroll wheel is being used and zooming is allowed, then zoom into the target GameObject!
            if ( Input.GetAxisRaw( "Mouse ScrollWheel" ) != 0f && allowZoom == true )
            {
                Zoom( zoomRate , minZoom , maxZoom );
            }

        }
        // has this message been already been printed?
        else if ( nullObjectMessageShown == false )
        {
            // If the target to follow does not exist (or has been destroyed).
            Debug.LogWarning( "Camera target is not defined or has been destroyed. \n@" +
                gameObject.name.ToString() + ".CameraFollow2D.FixedUpdate();" );
            // This message has already been printed!
            nullObjectMessageShown = true;
        }

    }

    /// <summary>
    /// Zooms into or out of the target, at a specified rate, and with a specified limit.
    /// </summary>
    public void Zoom( float rate , float minimumZoom , float maximumZoom )
    {
        // Make sure we aren't zooming more than the limit
        if ( cam.orthographicSize <= minimumZoom && cam.orthographicSize >= maximumZoom )
        {
            cam.orthographicSize -= rate * Input.GetAxis( "Mouse ScrollWheel" );
        }
        // If for some reason the camera zoomed out more than maxium, set it to the maxium.
        if ( cam.orthographicSize > minimumZoom )
        {
            cam.orthographicSize = minimumZoom;
        }
        // Same as above, but to make sure it didn't zoom int more than the minimum zoom. Set it to the minimum if so.
        else if ( cam.orthographicSize < maximumZoom )
        {
            cam.orthographicSize = maximumZoom;
        }
    }

    /// <summary>
    /// A smooth (slower) method to follow a target.
    /// </summary>
    private void SmoothFollow( Transform target )
    {
        Vector3 delta = target.position - cam.ViewportToWorldPoint( new Vector3( 0.5f , 0.5f , 0 ) );
        Vector3 destination = transform.position + delta;
        transform.position = Vector3.SmoothDamp( transform.position , destination + camOffset , ref velocity , lockingTime );
    }
    /// <summary>
    /// A fast method to follow a target.
    /// This method works by making the camera be always at the same position (+ offsets) as the target.
    /// </summary>
    private void FastFollow( Transform target )
    {
        transform.position = target.position + camOffset;
    }

}
