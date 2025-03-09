using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Transform target;

    private bool posFlag = false;
    private Vector3 posVelocity = Vector3.zero;
    [Tooltip("오프셋")] public Vector3 posOffset;
    [Range(0f, 1f), Tooltip("위치 보간 시간")] public float posSmoothTime = 0.2F;

    private bool rotFlag = false;
    [Range(0f, 1f), Tooltip("회전 보간 속도")] public float rotSmoothSpeed = 5f;

    private void LateUpdate() {
        if (target == null) {
            return;
        }

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition() {
        Vector3 targetPosition = target.position + posOffset;

        if (posFlag == true) {
            transform.position = targetPosition;
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref posVelocity, posSmoothTime);
            return;
        }
        
        posFlag = true;
    }

    private void UpdateRotation() {
        if (rotFlag == true) {
            transform.rotation = target.rotation;
            return;
        }

        if (Quaternion.Angle(transform.rotation, target.rotation) > 1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotSmoothSpeed);
            return;
        }

        rotFlag = true;
    }

    private void ChangeTarget(Transform next) {
        target = next;
        posVelocity = Vector3.zero;
        posFlag = false;
        rotFlag = false;
    }
}
