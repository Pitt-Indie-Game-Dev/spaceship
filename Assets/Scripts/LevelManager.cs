using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading(string sceneName) {
        string x = GetActiveScene().name;
        // static IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        // {
        //     AsyncOperation asyncLoad = LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //     while (!asyncLoad.isDone) yield return null;

        //     Scene newScene = GetSceneByName(sceneName);
        //     SetActiveScene(newScene);
        // }
        // StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        LoadScene(sceneName);

        UnloadSceneAsync(x);

    }
}
