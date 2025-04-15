using UnityEngine;

public class CannonBall : MonoBehaviour
{
    float pushtime = 1f;      //피격 시 PC가 래그돌 되는 시간

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 2f);

        if (collision.gameObject.name.Contains("Player"))
        {
            collision.gameObject.transform.root.GetComponentInChildren<PawnRagdoll>().ragdollTime = pushtime;
            collision.gameObject.transform.root.GetComponentInChildren<PlayerPawn>().PlayerRagdollStart(Vector3.zero);
        }
    }
}
