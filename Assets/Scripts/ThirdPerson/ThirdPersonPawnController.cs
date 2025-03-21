using UnityEngine;

public class ThirdPersonPawnController : MonoBehaviour {
	public GameObject pawnPrefab;
	protected ThirdPersonPawn PawnInstance;

	public void CreatePawn() {
		if (!pawnPrefab.TryGetComponent<ThirdPersonPawn>(out var pawn)) {
			Debug.LogWarning("pawnPrefab에 Pawn 컴포넌트 없음");
			return;
		}
		
		GameObject instance = Instantiate(pawnPrefab);
		AttachToPawn(instance);
	}

	public ThirdPersonPawn GetPawn() {
		return PawnInstance;
	}
	
	public void AttachToPawn(GameObject instance) {
		ThirdPersonPawn pawn;
		if (instance.TryGetComponent(out pawn)) {
			PawnInstance = pawn;
			pawn.controller = this;
			/*
			pawn.ControllerUnposses += (unpossesPawn) => {
				if (unpossesPawn == PawnInstance) {
					DetachFromPawn();
				}
			};*/
			pawn.Possess(this);
			AttachPawn(pawn);
		}
		else {
			Debug.LogWarning("지정된 객체에 Pawn 컴포넌트 없음.");
		}
	}
	
	public void DetachFromPawn() {
		if (PawnInstance) {
			PawnInstance.UnPossess();
			PawnInstance.controller = null;
		}
		DetachPawn();
		PawnInstance = null;
	}

	protected virtual void AttachPawn(ThirdPersonPawn pawn) { }
	protected virtual void DetachPawn() { }
}
