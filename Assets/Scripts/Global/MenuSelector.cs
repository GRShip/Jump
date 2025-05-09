using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour {

    public void SelectNewGame() {
        GameManager.Instance.ResetPrefs();
        SceneLoadManager.Instance.LoadSceneAsync("asdasd");
    }

    public void SelectContinueGame() {
        SceneLoadManager.Instance.LoadSceneAsync("asdasd");
    }

    public void SelectExitGame() {
        Application.Quit();
    }
}
