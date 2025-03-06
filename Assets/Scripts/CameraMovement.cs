using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Transform target;
    
    private bool posFlag = false;
    private Vector3 posVelocity = Vector3.zero;
    [Tooltip("오프셋")]
    public Vector3 posOffset;
    [Range(0f, 1f), Tooltip("위치 보간 시간")]
    public float posSmoothTime = 0.2F;
    
    private bool rotFlag = false;
    [Range(0f, 1f), Tooltip("회전 보간 속도")]
    public float rotSmoothSpeed = 5f;

    // Update is called once per frame
    void LateUpdate() {
        if (target != null) {
            Vector3 position = target.position + posOffset;

            if (posFlag == true) {
                transform.position = position;
                posVelocity = Vector3.zero;
            }
            else {
                if (Vector3.Distance(transform.position, position) < 0.1f) {
                    posFlag = true;
                }
                else {
                    transform.position = Vector3.SmoothDamp(transform.position, position, ref posVelocity, posSmoothTime);
                }
            }

            if (rotFlag == true) {
                transform.rotation = target.rotation;
            }
            else {
                if (Quaternion.Angle(transform.rotation, target.rotation) < 1f) {
                    rotFlag = true;
                }
                else {
                    transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotSmoothSpeed);
                }
            }
        }
        else {
            posFlag = false;
            rotFlag = false;
        }
    }
}
