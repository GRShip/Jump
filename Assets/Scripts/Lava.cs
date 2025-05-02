using UnityEngine;

public class Lava : MonoBehaviour
{
    float pushtime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.name.Contains("Player"))
        {
            other.gameObject.transform.root.GetComponentInChildren<PawnRagdoll>().ragdollTime = pushtime;
            other.gameObject.transform.root.GetComponentInChildren<PlayerPawn>().PlayerRagdollStart(Vector3.back);
        }
    }
}
