using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Rigidbody cannonball;
    public Transform fireTransform;

    float time = 0;
    public float cooltime = 5f;
    public float power = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= cooltime)
        {
            Rigidbody balloonInstance = Instantiate(cannonball, fireTransform.position, fireTransform.rotation);
            balloonInstance.AddForce(transform.forward.normalized * power, ForceMode.Impulse);
            time = 0;
        }
    }
}
