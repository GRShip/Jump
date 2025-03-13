#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public static SceneLoader Instance;
    
    private string sceneName = string.Empty;
    private bool sceneChange = false;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        sceneName = SceneStartInfo.ActiveEditorSceneName;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void Start() {
#if UNITY_EDITOR
        LoadSceneAsync(sceneName);
#else
        LoadSceneAsync("TitleScene");
#endif
    }

    public void SetTargetScene(string sceneName) {
        if (sceneChange == false) {
            this.sceneName = sceneName;
        }
        else {
            Debug.LogWarning("Not allowed while doing LoadSceneAsync");
        }
    }

    public void LoadScene(string sceneName) {
        this.sceneName = sceneName;
        SceneManager.LoadScene(this.sceneName);
    }
    
    public void LoadSceneAsync(string sceneName) {
        this.sceneName = sceneName;
        sceneChange = true;
        SceneManager.LoadScene("LoadingScene");
    }
    
    private void SceneLoaded(Scene scene, LoadSceneMode mode) {
        if (SceneManager.GetActiveScene().name != "LoadingScene") return;
        StartCoroutine(WaitSeconds(1f));
    }
    
    private IEnumerator WaitSeconds(float sec) {
        yield return new WaitForSeconds(sec);
        StartCoroutine(LoadTargetSceneAsync());
    }
    
    private IEnumerator LoadTargetSceneAsync() {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
        asyncOperation.allowSceneActivation = true;
        do {
            yield return null;
        } while (!asyncOperation.isDone);
        
        /*
        asyncOperation.allowSceneActivation = false;
        float elapsedTime = 0f;
        do {
            elapsedTime += Time.deltaTime;
            yield return null;
        } while (asyncOperation.progress < 0.9f || elapsedTime < 10f);
        asyncOperation.allowSceneActivation = true;
        */
        
        yield return asyncOperation;
        sceneChange = false;
    }
}

public static class SceneStartInfo {
    public static string ActiveEditorSceneName { get; private set; }

    public static void SetActiveEditorSceneName(string name) {
        ActiveEditorSceneName = name;
    }
}
