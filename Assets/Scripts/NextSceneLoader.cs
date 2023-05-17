using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NextSceneLoader : MonoBehaviour
{

    private bool isLoadingScene = false;

    public void loadNextScene()
    {
        // If the scene is already being loaded, return to avoid multiple simultaneous loads
        if (isLoadingScene)
        {
            return;
        }

        isLoadingScene = true;

        string currentSceneName = SceneManager.GetActiveScene().name;
        string sceneToLoad;

        switch (currentSceneName)
        {
            case "Fitts Law Circle Scene":
                sceneToLoad = "Stroop Room Scene";
                break;
            case "Stroop Room Scene":
                sceneToLoad = "Keyboard Type Scene";
                break;
            default:
                sceneToLoad = currentSceneName;
                break;
        }

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);

        // Reset the flag after the scene is loaded
        isLoadingScene = false;
    }

}
