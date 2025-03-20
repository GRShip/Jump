using UnityEngine;

public class CannonBall : MonoBehaviour
{
    float pushtime = 0.5f;      //피격 시 PC가 래그돌 되는 시간

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
    }
}
