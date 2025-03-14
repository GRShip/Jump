using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonPawn : MonoBehaviour, IPawn {
    public ThirdPersonController PlayerManager { get; set; }   
    public Action<GameObject> OnDestroyed;
    
    private PlayerInput playerInput;
    private ThirdPersonRagdoll tpRagdoll;
    
    [Header("Temp")]
    [Tooltip("설정")]
    //public int hpMax = 100;
    //private int _hp = 100;
    
    public ThirdPersonSpringArm cameraPosition;

    public bool HasController { get; private set; }

    private void Awake() {
        PlayerManager = null;
        playerInput = GetComponent<PlayerInput>();
        tpRagdoll = transform.Find("Mannequin").GetComponent<ThirdPersonRagdoll>();
        
        HasController = false;
        playerInput.enabled = false;
    }

    private void Update() {
        //Debug
        if (Input.GetMouseButtonDown(0) && HasController == true) {
            Damaged(Vector3.zero);
        }
        
        if (Input.GetMouseButtonDown(1) && HasController == true) {
            PlayerDestroy();
        }
    }

    public void PlayerDestroy() {
        tpRagdoll.SetRagdollState(true);
        UnPossess();
    }

    public void Possess(ThirdPersonController controller) {
        PlayerManager = controller;
        HasController = true;
        playerInput.enabled = true;
    }

    public void UnPossess() {
        PlayerManager = null;
        HasController = false;
        playerInput.enabled = false;
        SetPawnActivity(false);
        
        if (OnDestroyed != null) {
            OnDestroyed(gameObject);
            OnDestroyed = null;
        }
    }

    public void SetPawnActivity(bool newActivity) {
        IPawnComponent[] functionComponents = GetComponentsInChildren<IPawnComponent>();
        
        foreach (IPawnComponent component in functionComponents) {
            if (newActivity) {
                component.Active();
            }
            else {
                component.DeActive();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("SavePoint")) {
            if (HasController == true) {
                Debug.Log("SavePoint");
                ThirdPersonController.Instance.SavePosition(other.gameObject.transform);
            }
        }
    }

    public void Damaged(Vector3 force) {
        cameraPosition.SetRagdoll(tpRagdoll.hips);
        tpRagdoll.SetRagdollState(true);
        SetPawnActivity(false);
        StopAllCoroutines();
        StartCoroutine(ResetRagdoll(5f));
    }

    public IEnumerator ResetRagdoll(float time) {
        yield return new WaitForSeconds(time);
        tpRagdoll.SetRagdollState(false);
        
        

        Ray ray = new Ray(tpRagdoll.GetRagdollTransform().position, Vector3.down);
        RaycastHit hit;
        =
        //Spherecast로 바꾸기
        if (Physics.Raycast(tpRagdoll.GetRagdollTransform().position, Vector3.down, out hit, 5f, 1 << 7)) {
        //if (Physics.Raycast(ray, out hit, 1f)) {}
            transform.position = hit.point;
        }
        else {
            transform.position = tpRagdoll.GetRagdollTransform().position;
        }
        cameraPosition.ResetRagdoll();
        tpRagdoll.transform.localPosition = Vector3.zero;
        SetPawnActivity(true);
    }
}
