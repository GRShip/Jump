using UnityEngine;

public class CannonBall : MonoBehaviour
{
    float pushtime = 0.5f;      //�ǰ� �� PC�� ���׵� �Ǵ� �ð�

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
