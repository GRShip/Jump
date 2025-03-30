using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour {
    public Image image;
    public TMP_Text text;
    private bool Act;
    private void Start() {
        image = GetComponentsInChildren<Image>()[0];
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        SetEnable(false);
    }

    private void Update() {
        if (Act == false) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) == true) {
            GameManager.Instance.GameStart();
        }
    }

    public void SetEnable(bool value) {
        image.enabled = value;
        text.enabled = value;
        Act = value;
    }
}
