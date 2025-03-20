using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float speed = 2;
    float pushtime = 0.5f;      //피격 시 PC가 래그돌 되는 시간

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed); //풍선 전진
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); //풍선 파괴
    }

    //플레이어캐릭터 피격 시 플레이어 캐릭터의 피격 시 pushtime 넣어서 실행
}
