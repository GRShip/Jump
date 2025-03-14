using UnityEngine;

public class BearAttack : MonoBehaviour
{
    public Bear bfsm;

    public void PlayerHit()
    {
        bfsm.AttackAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
