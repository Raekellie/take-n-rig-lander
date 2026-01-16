using System.Collections;
using UnityEngine;

[RequireComponent ( typeof ( Rigidbody2D ) )]
public class PlayerController : MonoBehaviour
{
    // Player SpriteRenderer
    public SpriteRenderer playerRenderer;

    // Sprites
    // Note: the names refer to the thruster that is being used, not the direction the sprite should be used to go
    // Note 2: Idle is considered the same as OFF (this applied to the main thruster)
    public Sprite spriteOffOffOff; // All thrusters OFF
    public Sprite spriteOnOnOn; // All thrusters ON
    public Sprite spriteOffIdleOff; // Left and Right thrusters OFF, Main IDLE
    public Sprite spriteOffOnOff; // Left and Right thrusters OFF, Main IDLE
    public Sprite spriteOnIdleOff; // Left thruster ON, Main IDLE and Right OFF
    public Sprite spriteOnOnOff; // Left and Main thrusters ON, Right OFF
    public Sprite spriteOffIdleOn; // Right thruster ON, Main IDLE and Left OFF
    public Sprite spriteOffOnOn; // Right and Main thrusters ON, Left OFF

    // Explosion Prefab
    public GameObject explosionPrefab;
    // Explosion prefab animation duration
    public float explosionPrefabDuration;
    // Explosion Instance (to be able to destroy it)
    private GameObject explosionInstance;

    // Thrusters sound
    public AudioSource thrusterSound;
    // Explosion Sound
    public AudioSource explosionSound;

    // Engine state (ON - true / OFF - false)
    private bool engineState;
    // If the player is dead
    private bool isDead;

    // Thruster power (the power for the side thrusters is halved)
    public float thrusterPower;

    // Max velocity the player rocket can collide without exploding
    public float maxCollisionVelocity;

    // Player fuel tank size
    public float fuelTankSize;
    // Quantity of fuel spent per frame
    public float fuelRatio;
    // Player remaining fuel in the tank
    private float avaiableFuel;
    // If there is fuel left in the tank
    private bool tankEmpty;

    // Thrusters positions
    public Transform leftThruster;
    public Transform rightThruster;
    public Transform mainThruster;

    // Player RigidBody2D
    private Rigidbody2D playerBody;

    // Camera  - this will be used to get the maximum Z coordinate 
    //where GameObjects can be spawned without being invisible
    private Camera cam;

    // The value explained above will be stored in this variable
    private float cameraZaxis;

    // PopUp handler
    private PopUpText popUp;




    private void Start ()
    {
        // Get the PopUp handler component
        popUp = GameObject.FindGameObjectWithTag ( "GameController" ).GetComponent<PopUpText> ();
        if ( popUp != null ) popUp.ShowMessage ( 1 , 1f );
        popUp.ShowMessage ( 1 , 1f );
        // Disable the engine on start
        engineState = false;

        // Make the tank full on start
        tankEmpty = false;

        // Get the player RigidBody2D
        playerBody = GetComponent<Rigidbody2D> ();

        cam = Camera.main;
        cameraZaxis = cam.transform.position.z;

        // Enable Audio Sources
        explosionSound.enabled = true;
        thrusterSound.enabled = true;

    }

    private void FixedUpdate ()
    {
        // Start getting user input
        inputDetection ();

        // Debug the Axis values
        //Debug.Log ( "Horizontal Axis: " + Input.GetAxisRaw ( "Horizontal" ) );
        //Debug.Log ( "Vertical Axis: " + Input.GetAxisRaw ( "Vertical" ) );
    }

    /// <summary>
    /// Detects user input and does actions depending on it.
    /// </summary>
    private void inputDetection ()
    {

        // Toggle the Engine ON or OFF
        if ( isDead == false )
        {
            if ( Input.GetButtonDown ( "Engine" ) )
            {
                engineState = !engineState;
                if ( engineState == true )
                {
                    Debug.Log ( "Engine is ON" );

                    thrusterSound.Play ();
                }
                else
                {
                    Debug.Log ( "Engine is OFF" );

                    thrusterSound.Stop ();
                }
            }


            // If engine is OFF, set the sprite to OFF. If it is ON, set the sprite to ON_IDLE
            if ( engineState == false )
            {
                playerRenderer.sprite = spriteOffOffOff;
            }
            else
            {
                playerRenderer.sprite = spriteOffIdleOff;
            }

            // Only be able to move the rocket if the engine is ON
            if ( engineState == true && tankEmpty == false)
            {

                // Right and Up (go right with more speed - use main and left thrusters)
                if ( Input.GetAxisRaw ( "Horizontal" ) > 0 && Input.GetAxisRaw ( "Vertical" ) > 0 )
                {
                    playerRenderer.sprite = spriteOnOnOff;
                    Thrust ( thrusterPower , "RightUp" );

                }
                // Left and Up (go left with more speed - use main and right thrusters)
                else if ( Input.GetAxisRaw ( "Horizontal" ) < 0 && Input.GetAxisRaw ( "Vertical" ) > 0 )
                {
                    playerRenderer.sprite = spriteOffOnOn;
                    Thrust ( thrusterPower , "LeftUp" );

                }
                // Max power (use all thrusters)
                else if ( Input.GetAxisRaw ( "All Thrusters" ) > 0 )
                {
                    playerRenderer.sprite = spriteOnOnOn;
                    Thrust ( thrusterPower , "Max" );

                }
                // Up (go up)
                else if ( Input.GetAxisRaw ( "Vertical" ) > 0 )
                {
                    playerRenderer.sprite = spriteOffOnOff;
                    Thrust ( thrusterPower , "Up" );

                }
                // Right (go right - use left thruster)
                else if ( Input.GetAxisRaw ( "Horizontal" ) > 0 )
                {
                    playerRenderer.sprite = spriteOnIdleOff;
                    Thrust ( thrusterPower , "Right" );

                }
                // Left (go left - use right thruster)
                else if ( Input.GetAxisRaw ( "Horizontal" ) < 0 )
                {
                    playerRenderer.sprite = spriteOffIdleOn;
                    Thrust ( thrusterPower , "Left" );

                }

            }
        }

        //

    }

    /// <summary>
    /// Will use the RigidBody2D to move the rocket, according to thrusters position and power. This function does not change the sprite.
    /// </summary>
    /// <param name="amout">Amount(power) of thrust. Side thruster have their amount divided by 4.</param>
    /// <param name="direction">Max ; Up ; Left ; LeftUp ; Right ; RightUp</param>
    private void Thrust ( float amount , string direction )
    {
        // Main thruster will have a certain power
        // Each side thruster have half of the main thruster power
        // This results that if all are powered at the same time, the player (rocket) goes at one and a half on main thruster power
        switch ( direction )
        {

            case "Max":
                // The power here is multiplied by 2 to take into account the side thrusters
                playerBody.AddForceAtPosition ( this.transform.up * ( amount * 1.5f ) , mainThruster.position );
                break;
            case "Up":
                playerBody.AddForceAtPosition ( this.transform.up * amount , mainThruster.position );
                break;
            case "Left":
                playerBody.AddForceAtPosition ( this.transform.up * ( amount / 4 ) , rightThruster.position );
                break;
            case "LeftUp":
                // This calculates the point between the left and main thruster and applies the force (thrust) there
                playerBody.AddForceAtPosition ( this.transform.up * ( amount + amount / 4 ) , ( mainThruster.position + rightThruster.position ) / 2 );

                break;
            case "Right":
                playerBody.AddForceAtPosition ( this.transform.up * ( amount / 4 ) , leftThruster.position );
                break;
            case "RightUp":
                // This calculates the point between the right and main thruster and applies the force (thrust) there
                playerBody.AddForceAtPosition ( this.transform.up * ( amount + amount / 4 ) , ( mainThruster.position + leftThruster.position ) / 2 );
                break;

            default:
                Debug.LogError ( "The direction was not found or mispelled. \n@PlayerController.Thrust();" );
                break;
        }

    }

    /// <summary>
    /// Creates an explosion at a chosen position.
    /// </summary>
    /// <param name="explosionPosition">Where the explosion will happen. Leaving blank will make it happen on the center of the player.</param>
    private void Explode ( Vector2 explosionPosition )
    {

        if ( explosionPosition != new Vector2 ( 0 , 0 ) )
        {
            // This instantiates the explosion prefab at the position where the collision ocurred
            //The explosion prefab will also be a child of the player's GameObject
            explosionInstance = Instantiate ( explosionPrefab , new Vector3 ( explosionPosition.x , explosionPosition.y , cameraZaxis + 1 ) , Quaternion.identity );
        }
        else
        {

            // This instantiates the explosion prefab at the player position (at center).
            //The explosion prefab will also be a child of the player's GameObject
            explosionInstance = Instantiate ( explosionPrefab , new Vector3 ( transform.position.x , transform.position.y , cameraZaxis + 1 ) , Quaternion.identity );
        }

        //

    }


    private void OnCollisionEnter2D ( Collision2D col )
    {
        ContactPoint2D contactPoint = col.contacts [ 0 ];
        Vector2 contactPointPosition = contactPoint.point;

        if ( col.gameObject.tag != "Player"  && isDead == false)
        {
            Debug.Log ( "Velocity at collision: " + col.relativeVelocity.magnitude );

            if ( col.relativeVelocity.magnitude >= maxCollisionVelocity )
            {
                StartCoroutine ( destroyPlayer ( explosionPrefabDuration , contactPointPosition ) );

            }
        }
    }

    // If the only trigger on the player touches something, explode the rocket
    // That only trigger is supposed to be the fuselage where the pilot would be
    private void OnTriggerEnter2D ( Collider2D col )
    {
        // The raycast is used to determine where the collision ocurred, so the explosion happen there
        RaycastHit2D hit = Physics2D.Raycast ( transform.position , transform.forward );

        if ( col.gameObject.tag != "Player" && isDead == false)
        {
            StartCoroutine ( destroyPlayer ( explosionPrefabDuration , hit.transform.position ) );

        }
    }

    /// <summary>
    /// Waits a number of seconds.
    /// </summary>
    /// <param name="delay">Where the explosion will happen. Leaving blank (or null) will make it happen on the center of the player</param>
    /// <param name="explosionPosition">Where the explosion animation will happen.</param>
    /// <returns></returns>
    private IEnumerator destroyPlayer ( float delay , Vector2 explosionPosition )
    {
        // Disable everything, including player controls
        isDead = true;
        engineState = false;

        if ( popUp == null )
        {
            Debug.LogError ( "The GameManager does not have the tag \"GameController\". Can't display any message." );
        } else
        {
            popUp.ShowMessage ( 1 , 1f );
        }

        Debug.LogWarning ( "The rocket has exploded." );
        

        thrusterSound.Stop ();
        explosionSound.Play ();

        foreach ( Collider2D colliders in GetComponents<Collider2D> () )
        {
            colliders.enabled = false;
        }


        Explode ( explosionPosition );

        Destroy ( gameObject );

        // This will delay the destruction in X seconds
        yield  return new WaitForSeconds ( delay + 0.5f );
        Debug.Log ( "apos delay" );
        explosionSound.Stop ();
        Destroy ( explosionInstance );


    }


    /// <summary>
    /// Uses fuel
    /// </summary>
    /// <param name="thruster">Number of thrusters that are being used. Side thrusters count as 1, Main count as 2. (Max is 4)</param>
    private void useFuel (int thruster)
    {


        if(tankEmpty == true )
        {
            popUp.ShowMessage ( 2 , 1f );
            engineState = false;
            isDead = true;
        }
    }
}
