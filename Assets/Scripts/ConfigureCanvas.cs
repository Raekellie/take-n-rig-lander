using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables all canvas on level start, except the ones selected.
/// </summary>
public class ConfigureCanvas : MonoBehaviour
{

    public List<GameObject> canvasToKeepActive = new List<GameObject> ();

    private GameObject [] allCanvas;


    private void Awake ()
    {
        

        allCanvas = GameObject.FindGameObjectsWithTag ( "Canvas" );

        // Disables all canvas in scene
        for ( int i = 0 ; i < allCanvas.Length ; i++ )
        {

            allCanvas [ i ].gameObject.SetActive ( false );

        }

        // Activates the selected canvas
        for ( int i = 0 ; i < canvasToKeepActive.Count ; i++ )
        {

            canvasToKeepActive [ i ].SetActive ( true );

        }

    }


}
