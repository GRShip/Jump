using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour {

    public void SelectNewGame() {
        ThirdPersonController.Instance.ResetPrefs();
        SceneLoader.Instance.LoadSceneAsync("SampleScene");
    }

    public void SelectContinueGame() {
        SceneLoader.Instance.LoadSceneAsync("SampleScene");
    }

    public void SelectExitGame() {
        Application.Quit();
    }
}
