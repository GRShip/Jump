using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	[SerializeField]
	private int saveIndex = 0;
	
	[Tooltip("마우스잠금")]
	public bool cursorLocked = true;

	public GameState gameState { get; private set; }

	public UIGameOver uiGameOver;
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
			return;
		}
		
		//임시
		SceneManager.sceneLoaded += SceneLoaded;
		gameState = GameState.Noone;
	}

	private void OnDestroy() {
		//임시
		SceneManager.sceneLoaded -= SceneLoaded;
	}

	private void OnApplicationFocus(bool hasFocus) {
		SetCursorState(!cursorLocked);
	}

	public int GetSaveIndex() {
		return saveIndex;
	}
	
	private void SetCursorState(bool newState) {
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
	
	//임시
	private void SceneLoaded(Scene scene, LoadSceneMode mode) {
		if ((SceneManager.GetActiveScene().name != "SampleScene") && (SceneManager.GetActiveScene().name != "asdasd")) return;
		SetCursorState(cursorLocked);
		
		GameObject ui = GameObject.Find("UI_GameOver");
		uiGameOver = ui.GetComponent<UIGameOver>();
		if (uiGameOver) {
			uiGameOver.SetEnable(false);
		}
		GameStart();
	}
	
	public void GameStart() {
		gameState = GameState.Play;
		PlayerPawnController pc = GameObject.Find("PlayerManager").GetComponent<PlayerPawnController>();
		pc.CreatePawn();
		if (uiGameOver) {
			uiGameOver.SetEnable(false);
		}
		Debug.Log(pc.GetPawn().transform.position.ToString());
	}

	public void GameOver() {
		gameState = GameState.GameOver;
		if (uiGameOver) {
			uiGameOver.SetEnable(true);
		}
	}

	public void SavePosition(Transform tf, int index) {
		PlayerPrefs.SetFloat("SaveIndex", saveIndex);
		PlayerPrefs.SetFloat("PlayerPosX", tf.position.x);
		PlayerPrefs.SetFloat("PlayerPosY", tf.position.y);
		PlayerPrefs.SetFloat("PlayerPosZ", tf.position.z);
		
		PlayerPrefs.SetFloat("PlayerRotX", tf.eulerAngles.x);
		PlayerPrefs.SetFloat("PlayerRotY", tf.eulerAngles.y);
		PlayerPrefs.SetFloat("PlayerRotZ", tf.eulerAngles.z);

		PlayerPrefs.Save();
		saveIndex = index;
	}
	
	public Transform LoadPosition() {
		saveIndex = PlayerPrefs.GetInt("SaveIndex", 0);
		float posx = PlayerPrefs.GetFloat("PlayerPosX", 0);
		float posy = PlayerPrefs.GetFloat("PlayerPosY", 0);
		float posz = PlayerPrefs.GetFloat("PlayerPosZ", 0);
		
		float rotx = PlayerPrefs.GetFloat("PlayerRotX", 0);
		float roty = PlayerPrefs.GetFloat("PlayerRotY", 0);
		float rotz = PlayerPrefs.GetFloat("PlayerRotZ", 0);

		Transform tf = transform;
		tf.position = new Vector3(posx, posy, posz);
		tf.eulerAngles = new Vector3(rotx, roty, rotz);
		return tf;
	}
	
	[ContextMenu("ResetPlayerPrefs")]
	public void ResetPrefs() {
		PlayerPrefs.DeleteAll();
	}
}
