using UnityEngine;

public class ThirdPersonPawnController : MonoBehaviour {
	public GameObject pawnPrefab;
	protected GameObject PawnInstance;

	public void CreatePawn() {
		AttachToPawn(pawnPrefab);
	}
	
	private void AttachToPawn(GameObject instance) {
		PawnInstance = Instantiate(instance);
		if (PawnInstance == null) {
			Debug.LogWarning("Pawn Instantiate 실패");
			return;
		}
		
		ThirdPersonPawn pawn = PawnInstance.GetComponent<ThirdPersonPawn>();
		if (pawn) {
			pawn.PossessController(this);
			pawn.ControllerUnposses += (unpossesPawn) => {
				if (unpossesPawn == PawnInstance) {
					DetachFromPawn();
					pawn.ControllerUnposses = null;
				}
			};
			AttachPawn(pawn);
		}
		else {
			Debug.LogWarning("생성된 객체에 ThirdPersonPawn 컴포넌트가 없음.");
		}
	}
	
	private void DetachFromPawn() {
		PawnInstance = null;
		DetachPawn();
	}

	protected virtual void AttachPawn(ThirdPersonPawn pawn) { }
	protected virtual void DetachPawn() { }
}
