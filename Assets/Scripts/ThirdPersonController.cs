using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour {
	public static ThirdPersonController instance;
	
	public GameObject playerPrefab;
	
	[SerializeField]
	private GameObject _player;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(_player);
			return;
		}
	}

	private void Start() {
		GameStart();
	}
	
	public void GameStart() {
		CreatePlayer();
	}

	public void CreatePlayer() {
		_player = Instantiate(playerPrefab);
		if (_player == null) {
			Debug.LogWarning("플레이어 Instantiate 실패");
			return;
		}
		
		ThirdPersonPawn pawn = _player.GetComponent<ThirdPersonPawn>();
		if (pawn != null) {
			pawn.Possess(instance);
			pawn.onDestroyed += (destroyedInstance) => {
				// 파괴된 객체가 currentInstance인 경우에만 새 객체 생성
				if (destroyedInstance == _player) {
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
}