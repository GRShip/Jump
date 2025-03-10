using UnityEngine;

public class test : MonoBehaviour {
    public delegate void A(AnimationEvent animationEvent);
    A aDelegate;
    
    public delegate void B(AnimationEvent animationEvent);
    B bDelegate; 
    void Start() {
        aDelegate += transform.parent.GetComponent<PlayerMovement>().OnFootstep;
        bDelegate += transform.parent.GetComponent<PlayerMovement>().OnLand;
    }

    // Update is called once per frame
    void Update() { }
}
