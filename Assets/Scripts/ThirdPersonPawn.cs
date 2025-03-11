using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonPawn : MonoBehaviour {
    [Header("Temp")]
    [Tooltip("설정")]
    public int hpMax = 100;

    private int _hp = 100;

    public Action<GameObject> onDestroyed;

    public GameObject cameraPosition;

    private ThirdPersonController _playerManager;
    private PlayerInput _playerInput = null;
    private ThirdPersonRagdoll _tpRagdoll;

    public bool HasController { get; private set; }

    private void Awake() {
        _playerManager = null;
        _playerInput = GetComponent<PlayerInput>();
        _tpRagdoll = transform.Find("Mannequin").GetComponent<ThirdPersonRagdoll>();
        
        HasController = false;
        _playerInput.enabled = false;
    }

    private void Update() {
        //Debug
        if (Input.GetMouseButtonDown(1)) {
            PlayerDestroy();
        }
    }

    public void PlayerDestroy() {
        _tpRagdoll.SetRagdollState(true);
        Unpossess();
    }

    public void Possess(ThirdPersonController controller) {
        _playerManager = controller;
        HasController = true;
        _playerInput.enabled = true;
    }

    public void Unpossess() {
        _playerManager = null;
        HasController = false;
        _playerInput.enabled = false;
        
        if (onDestroyed != null) {
            onDestroyed(gameObject);
        }
    }
}
