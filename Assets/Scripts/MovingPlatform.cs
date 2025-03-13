using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("PlatformBlocker"))
        {
            speed *= -1;
        }
    }
}
