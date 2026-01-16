using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    private int currentSceneIndex;

    public GameObject inGameCanvas;
    public GameObject loadingScreen;
    public Slider loadingScreenSlider;
    public Button loadingScreenContinueButton;
    public Image loadingScreenContinueButtonBackground;

    public Text levelNameText;

    private Color defaultButtonBackgroundColor;
    public Color disabledButtonColor;

    AsyncOperation asyncOp;


    private PauseGame gamePauser;



    private void Start ()
    {
        // Get current scene index
        currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;

        // Default button color
        defaultButtonBackgroundColor = loadingScreenContinueButtonBackground.color;


        // Get the PauseGame script to be able to pause game while loading any scene
        gamePauser = GameObject.FindWithTag ( "GameController" ).GetComponent<PauseGame> ();


    }

    /// <summary>
    /// Reloads current scene.
    /// </summary>
    public void RestartCurrentScene ()
    {
        Time.timeScale = 1;
        StartCoroutine ( LoadAsynchronously ( currentSceneIndex, false ) );
    }

    /// <summary>
    /// Loads a scene.
    /// </summary>
    /// <param name="buildIndex">Scene to load build index.</param>
    public void LoadScene ( int sceneIndex )
    {
        if ( SceneManager.GetSceneByBuildIndex ( sceneIndex ).IsValid () == false )
        {
            Debug.LogError ( "The scene requested to load was not found. \n@SceneLoader.LoadScene(int)" );
        }
        else
        {
            StartCoroutine ( LoadAsynchronously ( sceneIndex , true) );
        }
    }
    /// <summary>
    /// Loads a scene.
    /// </summary>
    /// <param name="sceneName">Scene to load name.</param>
    public void LoadScene ( string sceneName )
    {
        int sceneToLoadIndex;

        if ( SceneManager.GetSceneByName ( sceneName ).IsValid () == false )
        {
            Debug.LogError ( "The scene requested to load was not found. \n@SceneLoader.LoadScene(string)" );
        }
        else
        {
            sceneToLoadIndex = SceneManager.GetSceneByName ( sceneName ).buildIndex;
            StartCoroutine ( LoadAsynchronously ( sceneToLoadIndex, true ) );
        }
    }

    /// <summary>
    /// Loads a scene Asynchronously
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    private IEnumerator LoadAsynchronously ( int sceneIndex, bool showLoadingScreen )
    {

        float progress;

        string levelName = SceneManager.GetSceneByBuildIndex ( sceneIndex ).name;

        asyncOp = SceneManager.LoadSceneAsync ( sceneIndex );
        asyncOp.allowSceneActivation = false;

        // Pause the game while loading any scene
        gamePauser.Pause (false);

        // If the loading screen is supposed to be shown,
        if ( showLoadingScreen == true )
        {
            // Disable default canvas and activate the loading screen - [UI]
            inGameCanvas.SetActive ( false );
            loadingScreen.SetActive ( true );

            // Show the user the name of the level that is being loaded
            levelNameText.text = levelName;


            // Disable the continue button
            loadingScreenContinueButton.interactable = false;
            loadingScreenContinueButtonBackground.color = disabledButtonColor;



            while ( asyncOp.isDone == false )
            {
                asyncOp.allowSceneActivation = false;

                progress = asyncOp.progress;

                loadingScreenSlider.value = progress;
                //float progress = Mathf.Clamp01 ( asyncOp.progress / 0.9f );
                Debug.Log ( progress );

                if ( progress == 0.9 || progress == 1 )
                {
                    // After the level has been loaded, activate the continue button and print a console message
                    Debug.LogWarning ( "Level " + levelName + " has been loaded!" );
                    loadingScreenContinueButton.interactable = true;
                    loadingScreenContinueButtonBackground.color = defaultButtonBackgroundColor;
                    // Unpause the game
                    gamePauser.Pause ( false );
                    break;
                }

                yield return null;
            }
        }
        // If the loading screen is not supposed to be shown,
        else
        {
            while ( asyncOp.isDone == false )
            {
                asyncOp.allowSceneActivation = true;


                //float progress = Mathf.Clamp01 ( asyncOp.progress / 0.9f );
                progress = asyncOp.progress;
                Debug.Log ( progress );

                if ( progress == 0.9 || progress == 1 )
                {
                    // After the level has been loaded, activate the continue button and print a console message
                    Debug.LogWarning ( "Level " + levelName + " has been loaded!" );
                    //UnPause the game
                    gamePauser.Pause ( false );
                    break;
                }

                yield return null;
            }
        }
    }


    /// <summary>
    /// Finish the loading process by entering the loaded scene.
    /// This should be called only after LoadScene() has executed.
    /// </summary>
    public void FinishLoad ()
    {
        asyncOp.allowSceneActivation = true;
    }

}