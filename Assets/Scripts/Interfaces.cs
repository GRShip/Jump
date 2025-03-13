using System;
using UnityEngine;

public interface IPawn {
    public ThirdPersonController PlayerManager { get; set; }
    public bool HasController { get; }
    
    public void Possess(ThirdPersonController controller);
    public void UnPossess();
}

public interface IPawnComponent {
    void DeActive();
    void Active();
}