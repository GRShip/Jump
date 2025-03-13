using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class SpringArm : MonoBehaviour, IPawnComponent {
	private Transform _arm;
	private Vector3 _armVelocity;

	[Min(0)] public float length = 5.0f;
	public float smoothTime = 0.5f;
	public float radius = 0.25f;
	public LayerMask collisionLayer = -1;
	
	private void Awake() {
		_arm = transform.Find("SpringArmTarget");
	}
	
	private void LateUpdate() {
		float armLength = GetLength();
		Vector3 armPosition = Vector3.back * armLength;
		
		_arm.localPosition = Vector3.SmoothDamp(_arm.localPosition, armPosition, ref _armVelocity, smoothTime);
	}

	public void OnWheel(InputValue input) {
		if (input.Get<float>() == 0) {
			return;
		}
		float value = input.Get<float>() > 0f ? -0.5f : 0.5f;
		length += value;
		length = Mathf.Clamp(length, 1, 10);
	}

	private float GetLength() {
		Ray ray = new Ray(transform.position, -transform.forward);
		RaycastHit hit;

		//구 캐스트
		if (Physics.SphereCast(ray, Mathf.Max(0.01f, radius), out hit, length, collisionLayer)) {
			return hit.distance;
		}
		return length;
	}
	
	private void OnDrawGizmos() {
		//임시
		if (_arm != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, _arm.position);
			Gizmos.DrawWireSphere(_arm.position, radius);
		}
	}
	
	public void DeActive() {
		enabled = false;
	}
	public void Active() {
		//throw new System.NotImplementedException();
	}
}