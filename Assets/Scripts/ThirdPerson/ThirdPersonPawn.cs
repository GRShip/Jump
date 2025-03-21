using UnityEngine;

public class ThirdPersonPawn : MonoBehaviour {
    public ThirdPersonPawnController controller = null;
    //public Action<GameObject> ControllerUnposses;
    private IPawnComponent[] pawnComponents;
    
    protected virtual void Awake() {
        pawnComponents = GetComponentsInChildren<IPawnComponent>();
    }
    
    public ThirdPersonPawnController GetController() {
        return controller;
    }
    
    public virtual void Possess(ThirdPersonPawnController ctrl) {}
    public virtual void UnPossess() {}

    protected void PawnComponentsActivity(bool newActivity) {
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
