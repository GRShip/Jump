using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class SpringArm : MonoBehaviour {
	private Transform arm;
	private Vector3 armVelocity;

	[Min(0)] public float length = 5.0f;
	public float smoothTime = 0.5f;
	public float radius = 0.25f;
	public LayerMask collisionMask = -1;
	
	private void Awake() {
		arm = transform.Find("SpringArmTarget");
	}
	
	private void LateUpdate() {
		float armLength = GetLength();
		Vector3 armPosition = Vector3.back * armLength;
		
		arm.localPosition = Vector3.SmoothDamp(arm.localPosition, armPosition, ref armVelocity, smoothTime);
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

		if (Physics.SphereCast(ray, Mathf.Max(0.01f, radius), out hit, length, collisionMask)) {
			return hit.distance;
		}
		return length;
	}
	
	private void OnDrawGizmos() {
		if (arm != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, arm.position);
			Gizmos.DrawWireSphere(arm.position, radius);
		}
	}
}