using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmartCannon : MonoBehaviour
{
    public Rigidbody cannonball;
    public Transform fireTransform;
    DecalProjector projector;

    float time = 0;
    public float cooltime = 3f;
    public float power = 20f;
    public float sight = 10f;
    public GameObject player;

    enum cool
    {
        Active,
        Cool
    }

    cool iscool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projector = GetComponent<DecalProjector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iscool == cool.Active)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= sight)
            {
                time += Time.deltaTime;

                transform.rotation = Quaternion.LookRotation
                    ((player.transform.position - transform.position).normalized);

                Ray ray = new Ray(fireTransform.position, fireTransform.forward);

                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    Vector3 localPosition = transform.InverseTransformPoint(hitInfo.point);
                    projector.pivot = localPosition;
                }

                if (time >= cooltime)
                {
                    iscool = cool.Cool;
                    StartCoroutine(FireCannon());
                }
            }
            else
            {
                Rest();
            }
        }
    }

    IEnumerator FireCannon()
    {
        projector.pivot = Vector3.zero;

        yield return new WaitForSeconds(1f);
        Rigidbody balloonInstance = Instantiate(cannonball, fireTransform.position, fireTransform.rotation);
        balloonInstance.AddForce(transform.forward.normalized * power, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        time = 0;
        iscool = cool.Active;
    }

    void Rest()
    {
        time = 0;
        projector.pivot = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
    }
}
