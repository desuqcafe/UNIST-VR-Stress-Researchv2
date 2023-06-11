using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;
    private SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void OnNextScene()
    {
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.ActivateNextScene();
    }
}