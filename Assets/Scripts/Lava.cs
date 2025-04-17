using UnityEngine;

public class Lava : MonoBehaviour
{
    float pushtime = 3f;

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
        Debug.Log("asd");
        if (other.transform.root.name.Contains("Player"))
        {
            other.gameObject.transform.root.GetComponentInChildren<PawnRagdoll>().ragdollTime = pushtime;
            other.gameObject.transform.root.GetComponentInChildren<PlayerPawn>().PlayerRagdollStart(Vector3.zero);
        }
    }
}
