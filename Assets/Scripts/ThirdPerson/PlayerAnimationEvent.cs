using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FootstepEvent : UnityEvent<AnimationEvent> { }

[System.Serializable]
public class LandEvent : UnityEvent<AnimationEvent> { }

public class PlayerAnimationEvent : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawnController Controller { get; private set; }
    
    /*
    public delegate void Footstep(AnimationEvent animationEvent);
    Footstep delegateFootstep;
    public delegate void Land(AnimationEvent animationEvent);
    Land delegateLand;
    */
    
    [SerializeField]
    private FootstepEvent EventFootstep;

    [SerializeField]
    private LandEvent EventLand;
    
    void Start() {
        Controller = GetComponentInParent<PlayerPawn>().GetController();
        //delegateFootstep += transform.parent.GetComponent<PlayerMovement>().OnFootstep;
        //delegateLand += transform.parent.GetComponent<PlayerMovement>().OnLand;
    }
    
    public void OnFootstep(AnimationEvent animationEvent) {
        //delegateFootstep?.Invoke(animationEvent);
        EventFootstep?.Invoke(animationEvent);
    }

    public void OnLand(AnimationEvent animationEvent) {
        //delegateLand?.Invoke(animationEvent);
        EventLand?.Invoke(animationEvent);
    }
    
    public void DeActive() {
        enabled = false;
    }
    public void Active() {
        enabled = true;
    }
}
