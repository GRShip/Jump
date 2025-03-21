using UnityEngine;

public class ScreenRay : MonoBehaviour {
    [SerializeField] private float maxForce;

    [SerializeField] private float maxForceTime;

    private float timeButtonDown;

    private Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Update() {
        if ((Input.GetMouseButtonDown(0))) {
            timeButtonDown = Time.time;
        }

        if ((Input.GetMouseButtonUp(0))) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                PlayerTest c = hit.collider.GetComponentInParent<PlayerTest>();
                if (c) {
                    float mouseButtonDownDuration = Time.time - timeButtonDown;
                    float forcePercent = mouseButtonDownDuration / maxForceTime;
                    float forceMagnitude = Mathf.Lerp(0.0f, maxForce, forcePercent);

                    Vector3 forceDir = c.transform.position - cam.transform.position;
                    forceDir.y = 1;
                    forceDir.Normalize();

                    Vector3 force = forceDir * forceMagnitude;

                    c.TriggerRagdoll(force, hit.point);
                }
            }
        }
    }
}
