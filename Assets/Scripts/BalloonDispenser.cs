using UnityEngine;

public class BalloonDispenser : MonoBehaviour
{
    public Rigidbody balloon;
    public Transform fireTransform;

    float time = 0;
    public float cooltime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if ( time >= cooltime )
        {
            Rigidbody balloonInstance = Instantiate(balloon, fireTransform.position, fireTransform.rotation);

            time = 0;
        }
    }
}
