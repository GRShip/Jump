using System;
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
    
    public GameObject cameraPosition;

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
        TriggerChildFunctions();
        
        if (OnDestroyed != null) {
            OnDestroyed(gameObject);
            OnDestroyed = null;
        }
    }

    public void TriggerChildFunctions() {
        IPawnComponent[] functionComponents = GetComponentsInChildren<IPawnComponent>();
        
        foreach (IPawnComponent component in functionComponents) {
            component.DeActive();
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
}
