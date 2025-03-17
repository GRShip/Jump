using System;
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