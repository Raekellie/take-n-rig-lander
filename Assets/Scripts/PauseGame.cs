using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{

    public GameObject pauseMenuCanvas;


    /// <summary>
    /// Pauses or Unpauses the game.
    /// </summary>
    public void Pause ( bool showMenu )
    {

        // If game is unpaused, pause it
        if ( Time.timeScale == 1 )
        {
            Debug.LogWarning ( "Game is NOW at 0 speed. [PAUSED]" );
            Time.timeScale = 0;
            // Don't forget to pause the audio!
            AudioListener.pause = true;

            if ( showMenu == true )
            {
                pauseMenuCanvas.SetActive ( true );
            }

        }
        // If game is paused, unpause it
        else if ( Time.timeScale == 0 )
        {
            Debug.LogWarning ( "Game is NOW at normal speed." );
            Time.timeScale = 1;
            // Don't forget to pause the audio!
            AudioListener.pause = false;

            pauseMenuCanvas.SetActive ( false );

        }

    }
}
