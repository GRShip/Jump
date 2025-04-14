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
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 2f);

        if (collision.gameObject.name.Contains("Player"))
        {
            collision.gameObject.GetComponentInChildren<PawnRagdoll>().ragdollTime = pushtime;
            collision.gameObject.GetComponentInChildren<PlayerPawn>().PlayerRagdollStart(Vector3.zero);
        }
    }
}
