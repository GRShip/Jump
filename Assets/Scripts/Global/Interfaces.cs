using UnityEngine;

public interface IPawn {
    public ThirdPersonPawnController PlayerManager { get; }
    public bool HasController { get; }
    
    public void Possess(ThirdPersonPawnController controller);
    public void UnPossess();
}

public interface IPawnComponent {
    ThirdPersonPawnController Controller { get; }
    void DeActive();
    void Active();
}

public class BoneTransform {
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
}

public enum PlayerState {
    Idle,
    Ragdoll,
    LerpBones,
    Standup
}
