using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour {

    public void SelectNewGame() {
        GameManager.Instance.ResetPrefs();
        SceneLoadManager.Instance.LoadSceneAsync("SampleScene");
    }

    public void SelectContinueGame() {
        SceneLoadManager.Instance.LoadSceneAsync("SampleScene");
    }

    public void SelectExitGame() {
        Application.Quit();
    }
}
