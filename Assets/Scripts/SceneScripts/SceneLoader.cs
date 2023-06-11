using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    private List<SceneAsyncOperation> sceneAsyncOperations = new List<SceneAsyncOperation>();
    private int currentSceneIndex = 0;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName != SceneManager.GetActiveScene().name)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncOperation.allowSceneActivation = false;
                sceneAsyncOperations.Add(new SceneAsyncOperation(sceneName, asyncOperation));
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != SceneManager.GetActiveScene().name)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void ActivateNextScene()
    {
        if (currentSceneIndex < sceneAsyncOperations.Count)
        {
            if (currentSceneIndex > 0)
            {
                SceneManager.UnloadSceneAsync(sceneAsyncOperations[currentSceneIndex - 1].SceneName);
            }
            sceneAsyncOperations[currentSceneIndex].AsyncOperation.allowSceneActivation = true;
            currentSceneIndex++;
        }
    }

    private class SceneAsyncOperation
    {
        public string SceneName { get; private set; }
        public AsyncOperation AsyncOperation { get; private set; }

        public SceneAsyncOperation(string sceneName, AsyncOperation asyncOperation)
        {
            SceneName = sceneName;
            AsyncOperation = asyncOperation;
        }
    }
}
