using UnityEngine;
using UnityEngine.SceneManagement;//임시

public class ThirdPersonController : MonoBehaviour {
	public static ThirdPersonController Instance;
	
	public GameObject playerPrefab;
	private GameObject player;
	
	public Transform spawnPosition;
	
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
		spawnPosition = gameObject.transform;
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
		CreatePlayer();
	}
	
	public void GameStart() {
		CreatePlayer();
	}

	public void CreatePlayer() {
		LoadPosition();
		player = Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);
		if (player == null) {
			Debug.LogWarning("플레이어 Instantiate 실패");
			return;
		}
		
		ThirdPersonPawn pawn = player.GetComponent<ThirdPersonPawn>();
		if (pawn != null) {
			pawn.Possess(Instance);
			pawn.OnDestroyed += (destroyedInstance) => {
				// 파괴된 객체가 currentInstance인 경우에만 새 객체 생성
				if (destroyedInstance == player) {
					PlayerDestroyed();
				}
			};
			Camera.main.GetComponent<CameraMovement>().ChangeTarget(pawn.cameraPosition.transform);
		}
		else {
			Debug.LogWarning("생성된 플레이어에 ThirdPersonPawn 컴포넌트가 없음.");
		}
	}

	void PlayerDestroyed() {
		CreatePlayer();
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
	
	public void LoadPosition() {
		float posx = PlayerPrefs.GetFloat("PlayerPosX", 0);
		float posy = PlayerPrefs.GetFloat("PlayerPosY", 0);
		float posz = PlayerPrefs.GetFloat("PlayerPosZ", 0);
		
		float rotx = PlayerPrefs.GetFloat("PlayerRotX", 0);
		float roty = PlayerPrefs.GetFloat("PlayerRotY", 0);
		float rotz = PlayerPrefs.GetFloat("PlayerRotZ", 0);
		
		spawnPosition.position = new Vector3(posx, posy, posz);
		spawnPosition.eulerAngles = new Vector3(rotx, roty, rotz);
	}
	
	[ContextMenu("ResetPlayerPrefs")]
	public void ResetPrefs() {
		PlayerPrefs.DeleteAll();
	}
}
