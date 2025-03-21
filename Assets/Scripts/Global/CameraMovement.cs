using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour {
    public GameObject attach;

    private bool posFlag = false;
    private bool rotFlag = false;
    [Tooltip("오프셋")]
    public Vector3 posOffset;
    [Range(1f, 100f), Tooltip("보간 속도")]
    public float smoothSpeed = 20f;
    
    [Tooltip("상승최대각도")]
    public float forwardPitchTop = 60.0f;
    [Tooltip("하강최대각도")]
    public float forwardPitchBottom = -60.0f;
    [Tooltip("회전감도")]
    public float forwardRotationRate = 10f;
    private float forwardYaw = 0f;
    private float forwardPitch = 0f;
    
    private bool holdRotate = false;
    
    private Vector2 lookInput;

    private void LateUpdate() {
        if (!attach) {
            return;
        }

        UpdatePosition();
        //UpdateRotation();
        CameraRotation();
    }

    private void UpdatePosition() {
        Vector3 targetPosition = attach.transform.position + posOffset;

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
    
    private void UpdateRotation() {
        if (rotFlag == true) {
            transform.rotation = attach.transform.rotation;
            return;
        }

        if (Quaternion.Angle(transform.rotation, attach.transform.rotation) > 1f) {
            //transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotSmoothSpeed);
            transform.rotation = ExponentialSlerp(transform.rotation, attach.transform.rotation, smoothSpeed);
            return;
        }

        rotFlag = true;
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

    public void ChangeTarget(GameObject next, bool rotatehold) {
        attach = next;
        posFlag = false;
        rotFlag = false;
        holdRotate = rotatehold;
    }
    
    private void CameraRotation() {
        if (lookInput.sqrMagnitude >= 0.01f) {
            //마우스 이동
            forwardYaw += lookInput.x * Time.deltaTime * forwardRotationRate;
            forwardPitch += -lookInput.y * Time.deltaTime * forwardRotationRate;
        }
        
        //오버플로방지
        forwardYaw = Mathf.Clamp(forwardYaw, float.MinValue, float.MaxValue);
        forwardPitch = Mathf.Clamp(forwardPitch, forwardPitchBottom, forwardPitchTop);
        
        //회전
        transform.rotation = Quaternion.Euler(forwardPitch, forwardYaw, 0.0f);

        if (attach && holdRotate) {
            attach.transform.rotation = transform.rotation;
        }
    }

    public void OnLook(InputValue value) {
        lookInput = value.Get<Vector2>();
    }
}
