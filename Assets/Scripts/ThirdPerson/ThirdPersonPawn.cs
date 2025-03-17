using System;
using UnityEngine;

public class ThirdPersonPawn : MonoBehaviour {
    public ThirdPersonPawnController controller;
    public Action<GameObject> ControllerUnposses;
    IPawnComponent[] pawnComponents;
    
    public ThirdPersonPawnController GetController() {
        return controller;
    }
    
    protected virtual void Awake() {
        controller = null;
    }

    protected virtual void Start() {
        pawnComponents = GetComponentsInChildren<IPawnComponent>();
    }
    
    public void PossessController(ThirdPersonPawnController ctrl) {
        controller = ctrl;
        Possess(ctrl);
    }

    public void UnPossessController() {
        controller = null;
        if (ControllerUnposses != null) {
            ControllerUnposses(gameObject);
        }
        UnPossess();
    }

    protected virtual void Possess(ThirdPersonPawnController ctrl) {}
    protected virtual void UnPossess() {}

    protected void ChangePawnActivity(bool newActivity) {
        foreach (IPawnComponent component in pawnComponents) {
            if (newActivity) {
                component.Active();
            }
            else {
                component.DeActive();
            }
        }
    }
}
