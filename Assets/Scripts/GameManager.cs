using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	
	public Transform PlayerSpawn { get; private set; }

	[Tooltip("마우스잠금")]
	public bool cursorLocked = true;

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
		PlayerSpawn = gameObject.transform;
	}

	private void OnDestroy() {
		//임시
		SceneManager.sceneLoaded -= SceneLoaded;
	}

	private void OnApplicationFocus(bool hasFocus) {
		SetCursorState(!cursorLocked);
	}

	private void SetCursorState(bool newState) {
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
	
	//임시
	private void SceneLoaded(Scene scene, LoadSceneMode mode) {
		if (SceneManager.GetActiveScene().name != "SampleScene") return;
		SetCursorState(cursorLocked);
		GameStart();
	}

	public void GameStart() {
		PlayerPawnController pc = GameObject.Find("PlayerManager").GetComponent<PlayerPawnController>();
		pc.CreatePawn();
	}

	public void SavePosition(Transform tf) {
		PlayerPrefs.SetFloat("PlayerPosX", tf.position.x);
		PlayerPrefs.SetFloat("PlayerPosY", tf.position.y);
		PlayerPrefs.SetFloat("PlayerPosZ", tf.position.z);
		
		PlayerPrefs.SetFloat("PlayerRotX", tf.eulerAngles.x);
		PlayerPrefs.SetFloat("PlayerRotY", tf.eulerAngles.y);
		PlayerPrefs.SetFloat("PlayerRotZ", tf.eulerAngles.z);

		PlayerPrefs.Save();
	}
	
	public Transform LoadPosition() {
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
