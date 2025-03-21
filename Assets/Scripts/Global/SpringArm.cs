using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class SpringArm : MonoBehaviour {
	public GameObject arm;
	private Vector3 armVelocity;
	private float armPrevious;
	private float armCurrent;

	[Min(0)] public float length = 5.0f;
	public float smoothTime = 0.5f;
	public float radius = 0.25f;
	public LayerMask collisionLayer = 7;
	
	private void LateUpdate() {
		armPrevious = armCurrent;
		armCurrent = GetLength();
		Vector3 armPosition = Vector3.back * armCurrent;

		if (armPrevious <= armCurrent) {
			arm.transform.localPosition = Vector3.SmoothDamp(arm.transform.localPosition, armPosition, ref armVelocity, smoothTime);
		}
		else {
			arm.transform.localPosition = armPosition;
		}
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
		if (arm != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, arm.transform.position);
			Gizmos.DrawWireSphere(arm.transform.position, radius);
		}
	}
	
	public void DeActive() {
		enabled = false;
	}
	public void Active() {
		enabled = true;
	}
}