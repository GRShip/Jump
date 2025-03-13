#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class DefaultSceneLoader {
    static DefaultSceneLoader() {
        var editorScene = EditorSceneManager.GetActiveScene().name;
        SceneStartInfo.SetActiveEditorSceneName(editorScene);

        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }
}
#endif