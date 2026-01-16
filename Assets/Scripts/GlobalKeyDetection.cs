using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for the global keys detection. (Not player controls)
/// </summary>
public class GlobalKeyDetection : MonoBehaviour
{

    PauseGame gamePauser;


    private void Start ()
    {
        
        gamePauser = GameObject.FindWithTag ( "GameController" ).GetComponent<PauseGame> ();

    }

    private void Update ()
    {
        // Detect if the pause key is pressed
        // If so, pause the game
        if ( Input.GetButtonDown ( "Pause" ) )
        {
            gamePauser.Pause ( true );
        }

    }



}
