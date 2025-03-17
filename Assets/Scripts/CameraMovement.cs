using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Transform target;

    private bool posFlag = false;
    private bool rotFlag = false;
    [Tooltip("오프셋")]
    public Vector3 posOffset;
    [Range(1f, 100f), Tooltip("보간 속도")]
    public float smoothSpeed = 20f;
    
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

        if (Vector3.Distance(transform.position, targetPosition) > 0.01f) {
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref posVelocity, posSmoothTime);
            transform.position = ExponentialLerp(transform.position, targetPosition, smoothSpeed);
            return;
        }
        
        posFlag = true;
    }

    Vector3 ExponentialLerp(Vector3 current, Vector3 target, float damping) {
        float t = 1 - Mathf.Exp(-damping * Time.deltaTime);
        return Vector3.Lerp(current, target, t);
    }

    Quaternion ExponentialSlerp(Quaternion current, Quaternion target, float damping) {
        // Time.deltaTime을 이용해 프레임 독립적인 t 값을 계산합니다.
        float t = 1 - Mathf.Exp(-damping * Time.deltaTime);
        return Quaternion.Slerp(current, target, t);
    }

    private void UpdateRotation() {
        if (rotFlag == true) {
            transform.rotation = target.rotation;
            return;
        }

        if (Quaternion.Angle(transform.rotation, target.rotation) > 1f) {
            //transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotSmoothSpeed);
            transform.rotation = ExponentialSlerp(transform.rotation, target.rotation, smoothSpeed);
            return;
        }

        rotFlag = true;
    }

    public void ChangeTarget(Transform next) {
        target = next;
        posFlag = false;
        rotFlag = false;
    }
}
