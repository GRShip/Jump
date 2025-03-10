using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float speed = 3;
    float pushtime = 0.5f;      //�ǰ� �� PC�� ���׵� �Ǵ� �ð�

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed); //ǳ�� ����
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); //ǳ�� �ı�
    }

    //�÷��̾�ĳ���� �ǰ� �� �÷��̾� ĳ������ �ǰ� �� pushtime �־ ����
}
