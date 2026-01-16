using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{

    public Text textBox;
    public Image textBoxBackground;

    public GameObject inGameCanvas;
    public GameObject popUpPrefab;

    private void Start ()
    {

        // Make the text and its background invisible on start
        textBox.enabled = false;
        textBoxBackground.enabled = false;

    }

    /// <summary>
    /// Displays a message to the player in a PopUp format. Messages already defined, chosen by a number.
    /// </summary>
    /// <param name="messageType">Defined messages:  "1": Death ; "2": No fuel left.</param>
    /// <param name="delayToDisappear">Delay before the text disappears.</param>
    public void ShowMessage ( int messageType , float delayToDisappear )
    {

        switch ( messageType )
        {
            case 1:
                DisplayPopUp ( "You exploded!" , delayToDisappear );
                break;
            case 2:
                DisplayPopUp ( "No fuel left!" , delayToDisappear );
                break;


            default:
                Debug.LogError ( "Message not defined or incorrectly spelled on call. \n@PopUpText.ShowMessage(int, float);" );
                break;
        }

    }

    /// <summary>
    /// Displays a message to the player in a PopUp format. Messages need to be written while calling this function.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="delayToDisappear">Delay before the text disappears.</param>
    public void ShowMessage ( string message , float delayToDisappear )
    {
        DisplayPopUp ( message , delayToDisappear );
    }

    private IEnumerator DisplayPopUp ( string text , float delayToDisappear )
    {

        /*popUpPrefab = */
        Instantiate ( popUpPrefab , inGameCanvas.transform.position , Quaternion.identity ); //, inGameCanvas.transform );

        textBox.enabled = true;
        textBoxBackground.enabled = true;

        textBox.text = text;





        Debug.LogError ( "before popup delay" );
        yield return new WaitForSeconds ( delayToDisappear );
        Debug.LogError ( "apos popup delay" );
        Destroy ( popUpPrefab );


        textBox.enabled = false;
        textBoxBackground.enabled = false;
    }


}
