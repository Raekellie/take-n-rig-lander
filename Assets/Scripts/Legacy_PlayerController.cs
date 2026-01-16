using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the main player controller
public class PlayerController_Legacy : MonoBehaviour
{
    // Main thruster power
    public float mainThrustForce;
    // Side thrusters thrust power (for ship maneuvering)
    public float sideThrustForce;

    // Player ship body
    private Rigidbody2D playerBody;

    // Side thusters Transforms (positions)
    private Transform leftSideThruster;
    private Transform rightSideThruster;
    // Main thuster Transforms (positions)
    private Transform mainThruster;

    // Heading direction
    private GameObject forceDirection;

    //If engine is powered ON
    private bool engineState;

    // Player SpriteRenderer
    private SpriteRenderer playerRenderer;

    // Sprites
    public Sprite spriteOff;
    public Sprite spriteOnIdle;
    public Sprite spriteOnNormal;
    public Sprite spriteOnMax;
    public Sprite spriteOnLeft;
    public Sprite spriteOnRight;
    public Sprite spriteOnNormalLeft;
    public Sprite spriteOnNormalRight;


    private void Start ()
    {
        // Disable the engine when the rocket is created
        engineState = false;

        // Get the player rigidbody
        playerBody = this.GetComponent<Rigidbody2D> ();

        // Get the player sprite renderer
        playerRenderer = this.gameObject.GetComponent<SpriteRenderer> ();

        // Get thrusters positions
        mainThruster = GameObject.Find ( "Main Thruster" ).transform;
        leftSideThruster = GameObject.Find ( "Left Thruster" ).transform;
        rightSideThruster = GameObject.Find ( "Right Thruster" ).transform;


        /*
         * Below there is code for debug purposes
         * (some were used only on the start of development but I decided to keep the code)
         */

        // Print thruster positions to see if something related to them is wrong
        Debug.Log ( "Thruster postions: \n" +
         "\n" + "Left thruster position: " + leftSideThruster.position.ToString () +
        "\n" + "Right Thruster position: " + rightSideThruster.position.ToString () +
        "\n" + "Main Thruster position: " + mainThruster.position.ToString () +
        "\n" );

        // Check if the playerBody variable is null
        // If yes, print a message warning that
        if ( playerBody == null )
        {
            Debug.LogError ( "The Player Ship doesn't have any RigidBody2D." );
        }
    }


    private void FixedUpdate ()
    {
        // Get the direction the player is heading
        forceDirection = GameObject.Find ( "Heading Direction" );

        // Detect the input
        detectInput ();

    }

    // Detect input
    private void detectInput ()
    {
        if ( Input.GetKeyDown ( KeyCode.E ) )
        {

            engineState = !engineState;

            if ( engineState == true )
            {
                Debug.Log ( "Engine ON" );
            }
            else
            {
                Debug.Log ( "Engine OFF" );
            }
            setSprite ( "off" );

        }
        // Only move and change sprites if the engine is ON
        if ( engineState == true )
        {
            // If in idle state (no controls) set sprite to idle
            movementController ( "idle" );
            
            // If W and A or UpArrow and LeftArrow are pressed...
            if ( Input.GetKey ( KeyCode.W ) && Input.GetKey ( KeyCode.A )
                ||
                Input.GetKey ( KeyCode.UpArrow ) && Input.GetKey ( KeyCode.LeftArrow ) )
            {
                movementController ( "onNormalRight" );
                
            }
            // If W and D or UpArrow and RightArrow are pressed...
            if ( Input.GetKey ( KeyCode.W ) && Input.GetKey ( KeyCode.D )
                ||
                Input.GetKey ( KeyCode.UpArrow ) && Input.GetKey ( KeyCode.RightArrow ) )
            {
                movementController ( "onNormalLeft" );
            }
            // If LeftShift or RightShift is pressed...
            else if ( Input.GetKey ( KeyCode.LeftShift ) || Input.GetKey ( KeyCode.RightShift ) )
            {
                movementController ( "max" );
            }
            // If W or UpArrow is pressed...
            else if ( Input.GetKey ( KeyCode.W ) || Input.GetKey ( KeyCode.UpArrow ) )
            {
                movementController ( "onNormal" );
            }
            // If D or RightArrow is pressed...
            else if ( Input.GetKey ( KeyCode.D ) || Input.GetKey ( KeyCode.RightArrow ) )
            {
                movementController ( "onLeft" );
            }
            // If A or LeftArrow is pressed...
            else if ( Input.GetKey ( KeyCode.A ) || Input.GetKey ( KeyCode.LeftArrow ) )
            {
                movementController ( "onRight" );
            }

        }
        
        else if ( engineState == false )
        {
            movementController ( "off" );
        }
    }

    // teste para saber velocidades, pode ser apagado a qualquer momento + o "if" relacionado
    bool executou = false;

    /// <summary>
    /// Movement controller. Will also determine what sprite has to be set , and set it.
    /// </summary>
    /// <param name="_action">
    /// off ; onIdle ; onNormal ; onMax ; onLeft ; onRight ; onNormalLeft ; onNormalRight
    /// </param>
    private void movementController ( string _action )
    {

        var heading = forceDirection.transform.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        //      |
        //     /\
        //   /    \
        // =====
        if ( executou == false )
        {
            Debug.Log ( "Max power: " + heading * ( mainThrustForce * sideThrustForce ) );
            Debug.Log ( "Main Thruster + 1 Side Thruster power: " + heading * ( mainThrustForce * ( sideThrustForce / 2 ) ) );
            Debug.Log ( "Main Thruster + 2 Side Thrusters power: " + heading * ( mainThrustForce * sideThrustForce ) );
            Debug.Log ( "Main Thruster power: " + heading * mainThrustForce );
            executou = true;
        }

        Vector2 axisInput = new Vector2 ( Input.GetAxisRaw ( "Horizontal" ) , Input.GetAxisRaw ( "Vertical" ) ).normalized;

        switch ( _action )
        {
            case "off":
                setSprite ( "off" );

                break;
            case "idle":
                setSprite ( "onIdle" );

                break;
            case "max":
                setSprite ( "onMax" );
                playerBody.AddForceAtPosition ( direction * ( mainThrustForce + ( sideThrustForce * 2 ) ) , mainThruster.position );

                break;
            case "onNormal":
                setSprite ( "onNormal" );
                playerBody.AddForceAtPosition ( direction * mainThrustForce , mainThruster.position );

                break;
            case "onLeft":
                setSprite ( "onLeft" );
                playerBody.AddForceAtPosition ( direction * ( sideThrustForce * 2 ) , leftSideThruster.position );

                break;
            case "onRight":
                setSprite ( "onRight" );
                playerBody.AddForceAtPosition ( direction * ( sideThrustForce * 2 ) , rightSideThruster.position );

                break;
            case "onNormalLeft":
                setSprite ( "onNormalLeft" );
                //playerBody.AddForceAtPosition ( direction * ( mainThrustForce + sideThrustForce ), mainThruster.position - rightSideThruster.position );
                playerBody.AddForceAtPosition ( axisInput * (mainThrustForce + sideThrustForce) , mainThruster.position - rightSideThruster.position );
                playerBody.AddForceAtPosition ( direction * ( mainThrustForce + sideThrustForce ) , /*mainThruster.position -*/ rightSideThruster.position );

                break;
            case "onNormalRight":
                setSprite ( "onNormalRight" );
               // playerBody.AddForceAtPosition ( direction * ( mainThrustForce + sideThrustForce ),mainThruster.position - leftSideThruster.position );
                //playerBody.AddForceAtPosition ( axisInput * ( mainThrustForce + sideThrustForce ) , mainThruster.position - leftSideThruster.position );
                playerBody.AddForceAtPosition ( direction * ( mainThrustForce + sideThrustForce ) , /*mainThruster.position -*/ leftSideThruster.position );

                break;

            default:
                Debug.LogError ( "The action requested was not chosen or incorrectly spelled on its call." + "\n" +
                "@ PlayerController.movementController();" );
                break;

        }

    }

    /// <summary>
    ///  Change the player sprite
    /// </summary>
    /// <param name="_sprite">
    /// off ; onIdle ; onNormal ; onMax ; onLeft ; onRight ; onNormalLeft ; onNormalRight
    /// </param>
    private void setSprite ( string _sprite )
    {

        // Change the sprites depending on what was chosen during the function call
        switch ( _sprite )
        {
            case "off":
                playerRenderer.sprite = spriteOff;
                break;
            case "onIdle":
                playerRenderer.sprite = spriteOnIdle;
                break;
            case "onNormal":
                playerRenderer.sprite = spriteOnNormal;
                break;
            case "onMax":
                playerRenderer.sprite = spriteOnMax;
                break;
            case "onLeft":
                playerRenderer.sprite = spriteOnLeft;
                break;
            case "onRight":
                playerRenderer.sprite = spriteOnRight;
                break;
            case "onNormalLeft":
                playerRenderer.sprite = spriteOnNormalLeft;
                break;
            case "onNormalRight":
                playerRenderer.sprite = spriteOnNormalRight;
                break;

            // If the sprite was spelled incorrectly during function call, show an error
            default:
                Debug.LogError ( "The sprite could not be changed because it does not exist or its name" + "\n" +
                    " was spelled incorrectly." +
                    "@ PlayerController.setSprite();" );
                break;
        }
    }
}
